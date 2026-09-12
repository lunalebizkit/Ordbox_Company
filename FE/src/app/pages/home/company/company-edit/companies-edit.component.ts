import { Component, ElementRef, Input, OnInit, signal, ViewChild } from "@angular/core";
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { NzMessageService } from "ng-zorro-antd/message";
import { NzNotificationService } from "ng-zorro-antd/notification";
import { BaseComponent } from "../../../../common/components/base/base.component";
import { HeaderOperationsButtonsComponent } from "../../../../common/components/headers/buttons.oparations.header.component";
import { PopupConfirmationComponent } from "../../../../common/components/popup-confirmation/popup-confirmation.component";
import { NzSpinModule } from "ng-zorro-antd/spin";
import { NzCollapseModule } from "ng-zorro-antd/collapse";
import { NzFormModule } from "ng-zorro-antd/form";
import { NzSelectModule } from "ng-zorro-antd/select";
import { NzButtonModule } from "ng-zorro-antd/button";
import { NzInputModule } from "ng-zorro-antd/input";
import { ActivatedRoute, Router } from "@angular/router";
import { Permission } from "../../../../common/auth/models/permissions.enum";
import { CompanyService } from "../companies.services";
import { CompanyModel } from "../model/company.model";
import { NzSwitchModule } from "ng-zorro-antd/switch";
@Component({
    selector: 'app-companies.edit',
    templateUrl: './companies-edit.component.html',
    imports: [HeaderOperationsButtonsComponent, PopupConfirmationComponent, NzSpinModule, NzCollapseModule, NzFormModule, ReactiveFormsModule, NzSelectModule, NzButtonModule, NzInputModule, NzSwitchModule]
})

export class CompaniesEditComponent extends BaseComponent implements OnInit {

    permissions = Permission;

    @ViewChild('header') headerComponent!: HeaderOperationsButtonsComponent;
    @ViewChild('popup') popupComponent!: PopupConfirmationComponent;
    @ViewChild('pop') popComponent!: PopupConfirmationComponent;
    /*
   ** Determina si esta en proceso de guardado
   */
    isSaving = signal<boolean>(false);
    isEditMode = signal<boolean>(false);

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
     ** Determina si se muestra la sección para cambio de companyEmailPass
     */
    showPasswordChange = true;
    /*
  ** Formulario
  */
    form!: FormGroup;

    constructor(
        private service: CompanyService,
        notificacionService: NzNotificationService,
        el: ElementRef,
        message: NzMessageService,
        private fb: FormBuilder,
        private route: ActivatedRoute,
        private router: Router,
    ) {
        super(notificacionService, el, message);
        this.form = this.fb.group({
            companyName: [{ value: '', disabled: true }, [Validators.required]],
            companyOwnerName: [{ value: '', disabled: true }, [Validators.required]],
            companyAddress: [{ value: '', disabled: true }, [Validators.required]],
            companyDescription: [{ value: '', disabled: true }, [Validators.required]],
            companyCuit: [{ value: '', disabled: true }, [Validators.required]],
            companyPoint: [{ value: '', disabled: true }, [Validators.required]],
            companyEmailPass: [{ value: '', disabled: true },],
            checkpassword: [{ value: '', disabled: true },],
            companyEmail: [{ value: '', disabled: true }, [Validators.email]],
            isDeleted: [{ value: false, disabled: true }, [Validators.required]],

        })
    }

    ngOnInit(): void {
        this.route.params.subscribe({
            next: (p) => {
                if (p['id']) {
                    this.isLoading.set(true);
                    this.getData(p['id']);
                }
            },
            error: () => { }
        });

    };

    getData(id: number): void {
        if (id != 0) {
            this.id.set(id);
            this.service.getById(id).subscribe({
                next: (r) => {
                    this.form.controls['companyName'].setValue(r.companyName),
                        this.form.controls['companyOwnerName'].setValue(r.companyOwnerName),
                        this.form.controls['companyAddress'].setValue(r.companyAddress),
                        this.form.controls['companyDescription'].setValue(r.companyDescription),
                        this.form.controls['companyCuit'].setValue(r.companyCuit)
                        this.form.controls['companyEmail'].setValue(r.companyEmail),
                        this.form.controls['companyEmailPass'].setValue(r.companyEmailPass),
                        this.form.controls['companyPoint'].setValue(r.companyPoint),
                        this.form.controls['isDeleted'].setValue(r.isDeleted),
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
            const model: CompanyModel = {
                id: this.id(),
                companyName: this.form.controls['companyName'].value,
                companyOwnerName: this.form.controls['companyOwnerName'].value,
                companyCuit: this.form.controls['companyCuit'].value,
                companyAddress: this.form.controls['companyAddress'].value,
                companyEmailPass: this.form.controls['companyEmailPass'].value,
                companyEmail: this.form.controls['companyEmail'].value,
                companyDescription: this.form.controls['companyDescription'].value,
                isDeleted: this.form.controls['isDeleted'].value,
                companyPoint: this.form.controls['companyPoint'].value,                
            };
            this.isSaving.set(true);
            this.service.saveCompany(model)
                .subscribe({
                    next: (r) => {
                        this.showNotificationSuccess(
                            'Guardado correcto',
                            `Se edito correctamente el usuario ${model.companyName}`
                        );
                        this.isSaving.set(false);
                        this.router.navigate(['/home/companies'])
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
        this.form.controls['companyEmailPass'].setValidators([Validators.required]);
        this.form.get('companyEmailPass')!.updateValueAndValidity();
        this.form.controls['checkpassword'].setValidators([Validators.required, this.confirmationValidator]);
        this.form.get('checkpassword')!.updateValueAndValidity();

    }

    /*
     ** Comprueba que ambas contraseñas son iguales
     */
    updateConfirmValidator(): void {
        Promise.resolve().then(() => this.form.controls['companyEmailPass'].updateValueAndValidity());
        Promise.resolve().then(() => this.form.controls["checkpassword"].updateValueAndValidity());

    }

    confirmationValidator = (control: FormControl): { [s: string]: boolean } => {
        if (!control.value) {
            return { required: true };
        } else if (control.value !== this.form.controls['companyEmailPass'].value) {
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

    addCertificate() {
        if (this.id()>0){
            this.router.navigate([`/home/companycertificate/edit/${this.id()}`]);
        }
    }

}