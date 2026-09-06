
import { Component, ElementRef, OnInit, signal, ViewChild } from "@angular/core";
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { Router } from "@angular/router";
import { NzMessageService } from "ng-zorro-antd/message";
import { NzNotificationService } from "ng-zorro-antd/notification";
import { NzTransferModule, TransferDirection, TransferItem } from "ng-zorro-antd/transfer";
import { PermissionRolService } from "../permission-rol.service";
import { AddOrUpdatePermission, PermissionRol } from "./model/permission-rol.model";
import { BaseComponent } from "../../../../common/components/base/base.component";
import { HeaderOperationsButtonsComponent } from "../../../../common/components/headers/buttons.oparations.header.component";
import { PopupConfirmationComponent } from "../../../../common/components/popup-confirmation/popup-confirmation.component";
import { NzSpinModule } from "ng-zorro-antd/spin";
import { NzFormModule } from "ng-zorro-antd/form";
import { NzSelectModule } from "ng-zorro-antd/select";
import { NzCollapseModule } from "ng-zorro-antd/collapse";
import { Permission } from "../../../../common/auth/models/permissions.enum";

@Component({
    selector: 'app-permission-rol',
    templateUrl: './permission-rol.component.html',
    styleUrls: ['./permission-rol.component.css'],
    imports: [HeaderOperationsButtonsComponent, NzSpinModule, NzFormModule, PopupConfirmationComponent, NzSelectModule, NzTransferModule, ReactiveFormsModule, NzCollapseModule]
})

export class PermissionRolComponent extends BaseComponent implements OnInit {

    @ViewChild('header') headerComponent!: HeaderOperationsButtonsComponent;
    @ViewChild('popup') popupComponent!: PopupConfirmationComponent;

    isSaving = signal<boolean>(false);
    isLoading = signal<boolean>(false);
    disabled = signal<boolean>(true);
    // permissionRol = [];
    // permissionRolList: PermissionRol[] = [];
    list: TransferItem[] = [];
    // listComplete: TransferItem[] = [];
    /* Formulario  */
    form!: FormGroup;
    rolSelected = signal<number>(0);
    permissionIdList: number[] = [];

    permissionRol = signal<{ value: number, label: string }[]>([]);
    permissionRolList = signal<PermissionRol[]>([]);
    listComplete = signal<TransferItem[]>([]);

    permissions = Permission;
    isEditMode = signal<boolean>(false);

    constructor(
        private servicePermission: PermissionRolService,
        notificacionService: NzNotificationService,
        el: ElementRef,
        message: NzMessageService,
        private fb: FormBuilder,
        private router: Router,
    ) {
        super(notificacionService, el, message);
        this.form = this.fb.group({
            roleId: [{ value: '', disabled: true }, Validators.required],
            permissions: [{ value: [], disabled: true }]
        });
    }

    ngOnInit(): void {
        this.getPermission();
        this.getPermissionRol();
    }
    getPermission(): void {
        this.servicePermission.permissionList().subscribe({
            next: (r) => {
                // r.forEach((element: any) => {
                //     this.list.push({
                //         key: element.id, title: element.name, disabled: false
                //         , direction: 'left'
                //     })
                // });
                this.listComplete.set(r.map((element: any) => ({
                    key: element.id, title: element.name, disabled: false
                    , direction: 'left' as TransferDirection
                })
                ));

            },
            error: () => { }
        })
    }

    getPermissionRol(): void {
        this.servicePermission.permissionRolList().subscribe({
            next: (r) => {
                this.permissionRol.set(
                    r.map((rol: { id: number, rol: string }) => ({ value: rol.id, label: rol.rol }))
                );
                this.permissionRolList.set(r);
            },
            error: () => {
                this.permissionRol.set([]);
            }
        })
    };

    rolSelectedChange(id: any): void {
        this.rolSelected.set(id);
        this.renderOwnPermissions();
    };

    renderOwnPermissions(): void {
        // this.permissionIdList = [];
        // let newListOfPermissions: TransferItem[] = [];
        // let newListOfPermissionsRight: TransferItem[] = [];
        // let permission = this.permissionRolList.filter((item: any) => item.id == this.rolSelected)[0];

        // if (permission.permissions.length > 0) {

        //     permission.permissions.forEach(element => {
        //         newListOfPermissionsRight.push({ key: element.id, title: element.name, direction: 'right', disabled: false })
        //     });

        //     this.list.forEach((item, index) => {
        //         if (newListOfPermissionsRight.find((element) => element.title == item.title && element.direction === 'right')) {
        //             let newEditPermission: TransferItem = newListOfPermissionsRight.filter(p => p.title == item.title && p.direction != item.direction)[0];
        //             newEditPermission.direction = 'right';

        //             this.permissionIdList.push(newEditPermission['key']);

        //             newListOfPermissions.push(newEditPermission);
        //         }
        //         else {
        //             newListOfPermissions.push(item);
        //         }
        //     })
        //         ;
        //     this.listComplete = newListOfPermissions
        // } else {

        //     this.form.controls['permissions'].setValue(this.permissionIdList);
        // }
        this.permissionIdList = [];
        const permission = this.permissionRolList().find(item => item.id == this.rolSelected());

        if (permission && permission.permissions.length > 0) {
            const newList = this.listComplete().map(item => {
                const match = permission.permissions.find(p => p.name === item.title);
                if (match) {
                    this.permissionIdList.push(match.id);
                    return { ...item, direction: 'right' as TransferDirection };
                }
                return { ...item, direction: 'left' as TransferDirection };
            });

            this.listComplete.set(newList);
            this.form.controls['permissions'].setValue(this.permissionIdList);
        } else {
            const resetList = this.listComplete().map(item => ({
                ...item,
                direction: 'left' as TransferDirection
            }));

            this.listComplete.set(resetList);
            this.form.controls['permissions'].setValue([]);
            this.permissionIdList = [];
        }
    };

    msjConfirmOk() {
        try {
            if (this.isValidForm(this.form)) {
                this.popupComponent.showConfirmation()
            }
        } catch (error) {
            console.log(error);

        }
    };
    save() {
        const model: AddOrUpdatePermission =
        {
            id: this.form.controls['roleId'].value,
            name: '',
            key: '',
            permissionIds: this.form.controls['permissions'].value as number[],
        };
        this.servicePermission.addOrUpdatePermissions(model).subscribe({
            next: (r) => {
                this.showNotificationSuccess(
                    'Guardado correcto',
                    `Se guardo correctamente el Cambio`
                );
                this.isSaving.set(false)
                this.router.navigate(['/home/users']);
            },
            error: () => {
                this.isSaving.set(false);
                this.showMessageError('No se pudo Guardar el Cambio');
                this.router.navigate(['/home/users']);
            }
        })

    }

    change(ret: any): void {
        ret.list.forEach((element: { key: any, title: string, direction: string }) => {
            if (element.direction === 'right') {
                this.permissionIdList.push(Number(element.key))
            };
            if (element.direction === 'left') {
                this.permissionIdList = this.permissionIdList.filter(number =>
                    number !== Number(element.key))
            }
            this.form.controls['permissions'].setValue([...this.permissionIdList]);
        });
    }

    toggleEdit() {
        this.isEditMode.set(!this.isEditMode());
        if (this.isEditMode()) {
            this.form.enable();
        } else {
            this.form.disable();
        }
        this.disabled.set(!this.isEditMode());
    }
}