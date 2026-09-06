import { Component, ElementRef, Input, OnInit, signal, ViewChild } from "@angular/core";
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { NzMessageService } from "ng-zorro-antd/message";
import { NzNotificationService } from "ng-zorro-antd/notification";
import { eRol, rolList } from "../model/rol.enum";
import { UserModel } from "../model/user.model";
import { UserService } from "../users.services";
import { TransferItem } from "ng-zorro-antd/transfer";
import { BaseComponent } from "../../../../common/components/base/base.component";
import { HeaderOperationsButtonsComponent } from "../../../../common/components/headers/buttons.oparations.header.component";
import { PopupConfirmationComponent } from "../../../../common/components/popup-confirmation/popup-confirmation.component";
import { PermissionRolService } from "../../permission-rol/permission-rol.service";
import { NzSpinModule } from "ng-zorro-antd/spin";
import { NzCollapseModule } from "ng-zorro-antd/collapse";
import { NzFormModule } from "ng-zorro-antd/form";
import { NzSelectModule } from "ng-zorro-antd/select";
import { NzButtonModule } from "ng-zorro-antd/button";
import { NzInputModule } from "ng-zorro-antd/input";
import { ActivatedRoute, Router } from "@angular/router";
import { Permission } from "../../../../common/auth/models/permissions.enum";
@Component({
    selector: 'app-users.edit',
    templateUrl: './users-edit.component.html',
    imports: [HeaderOperationsButtonsComponent, PopupConfirmationComponent, NzSpinModule, NzCollapseModule, NzFormModule, ReactiveFormsModule, NzSelectModule, NzButtonModule, NzInputModule]
})

export class UsersEditComponent extends BaseComponent implements OnInit {
    
    permissions = Permission;

    @ViewChild('header') headerComponent!: HeaderOperationsButtonsComponent;
    @ViewChild('popup') popupComponent!: PopupConfirmationComponent;
    @ViewChild('pop') popComponent!: PopupConfirmationComponent;
    /*
   ** Determina si esta en proceso de guardado
   */
    isSaving = signal<boolean>(false);
    isEditMode= signal<boolean>(false);

    /*
     ** Determina si esta buscando el usuario
     */
    isLoading = signal<boolean>(false);

    /*
     ** id del usuario a editar, si es nuevo...
     */
    id = signal<number>(0);

    /*
     ** Determina si se estan cargando los roles
     */
    isLoadingRoles = true;


    /*
     ** Determina si se muestra la sección para cambio de password
     */
    showPasswordChange = true;
    /*
  ** Formulario
  */
    form!: FormGroup;

    /*
     ** Listado de todos los roles
     */
    allRols = rolList;
    rolSelected: any;
    list: TransferItem[] = [];
    listComplete: TransferItem[] = [];
    permissionRol= signal<[]>([]);
    permissionRolList= signal<Permissions[]>([]);

    constructor(
        private service: UserService,
        private servicePermission: PermissionRolService,
        notificacionService: NzNotificationService,
        el: ElementRef,
        message: NzMessageService,
        private fb: FormBuilder,
        private route: ActivatedRoute,
        private router: Router,
    ) {
        super(notificacionService, el, message);
        this.form = this.fb.group({
            firstName: [{ value: '',disabled: true}, [Validators.required]],
            lastName: [{ value: '',disabled: true},[Validators.required]],
            userName: [{ value: '',disabled: true}, [Validators.required]],
            password: [{ value: '',disabled: true},],
            checkpassword: [{ value: '',disabled: true},],
            email: [{ value: '',disabled: true}, [Validators.email]],
            roleId: [{ value: '',disabled: true}, [Validators.required]],

        })
    }

    ngOnInit(): void {
        this.route.params.subscribe({
            next: (p) => {
                if (p['id']) {
                    this.isLoading.set(true);
                    this.getUser(p['id']);
                }
            },
            error: () => { }
        });
        
        this.getPermission();
        this.getPermissionRol();
    };
    getRolName(id: number) {
        return eRol[id];
    }
    getPermission(): void {
        this.servicePermission.permissionList().subscribe({
            next: (r) => {
                r.forEach((element: any) => {
                    this.list.push({
                        key: element.id, title: element.name, disabled: false
                        , direction: 'left'
                    })
                });
                this.listComplete = this.list;

            },
            error: () => { }
        })
    }
    getPermissionRol(): void {
        this.servicePermission.permissionRolList().subscribe({
            next: (r) => {
                this.permissionRol.set(r.map((rol: { id: number, rol: string }) => { return { value: rol.id, label: rol.rol } }));
                this.permissionRolList.set(r);
            },
            error: () => {
                this.permissionRol.set([]);
            }
        })
    };
    rolSelectedChange(id: any): void {
        this.rolSelected = id;
    };

    getUser(id: number): void {
        if (id != 0) {
            this.id.set(id);
            this.service.getById(id).subscribe({
                next: (r) => {
                    this.form.controls['firstName'].setValue(r.firstName),
                        this.form.controls['lastName'].setValue(r.lastName),
                        this.form.controls['userName'].setValue(r.userName),
                        this.form.controls['password'].setValue(r.password)
                    this.form.controls['email'].setValue(r.email),
                        this.form.controls['roleId'].setValue(r.roleId),
                        this.isLoading.set(false);
                },
                error: () => {
                    this.isLoading.set(false);
                }
            })
        }
    };

    save(): void {
        this.updateConfirmValidator();
        if (this.isValidForm(this.form)) {
            const model: UserModel = {
                id: this.id(),
                firstName: this.form.controls['firstName'].value,
                lastName: this.form.controls['lastName'].value,
                userName: this.form.controls['userName'].value,
                password: this.form.controls['password'].value,
                email: this.form.controls['email'].value,
                roleId: this.form.controls['roleId'].value,
            };
            this.isSaving.set(true);
            this.service.saveUser(model)
                .subscribe({
                    next: (r) => {
                        this.showNotificationSuccess(
                            'Guardado correcto',
                            `Se edito correctamente el usuario ${model.userName}`
                        );
                        this.isSaving.set(false);
                        this.router.navigate(['/home/users'])
                    },
                    error: () => {
                        this.isSaving.set(false);
                        this.showMessageError('No se pudo editar el usuario');
                    }
                })
        }
    };

    /*
  ** Muestra los inputs para cambiar la constraseña y los hace obligatorios
  */
    showPasswordChangeBox(): void {
        this.showPasswordChange = false;
        this.form.controls['password'].setValidators([Validators.required]);
        this.form.get('password')!.updateValueAndValidity();
        this.form.controls['checkpassword'].setValidators([Validators.required, this.confirmationValidator]);
        this.form.get('checkpassword')!.updateValueAndValidity();

    }

    /*
     ** Comprueba que ambas contraseñas son iguales
     */
    updateConfirmValidator(): void {
        Promise.resolve().then(() => this.form.controls['password'].updateValueAndValidity());
        Promise.resolve().then(() => this.form.controls["checkpassword"].updateValueAndValidity());

    }

    confirmationValidator = (control: FormControl): { [s: string]: boolean } => {
        if (!control.value) {
            return { required: true };
        } else if (control.value !== this.form.controls['password'].value) {
            return { confirm: true, error: true };
        }
        return {};
    }

    msjConfirmOk() {
        try {
            if (this.isValidForm(this.form)) {
                this.popComponent.showConfirmation();
            } else {
                this.showMessageError
            }
        } catch (error) {
            console.log(error);

        }
    }

    toggleEdit() {
    this.isEditMode.set(!this.isEditMode());
    if (this.isEditMode()) {
        this.form.enable();
    } else {
        this.form.disable();
    }
    }

}