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
import { CompanyCertificateModel, CompanyModel } from "../model/company.model";
import { NzSwitchModule } from "ng-zorro-antd/switch";
@Component({
    selector: 'app-company-certificate.edit',
    templateUrl: './company-certificate-edit.component.html',
    imports: [HeaderOperationsButtonsComponent, PopupConfirmationComponent, NzSpinModule, NzCollapseModule, NzFormModule, ReactiveFormsModule, NzSelectModule, NzButtonModule, NzInputModule, NzSwitchModule]
})

export class CompanyCertificateEditComponent extends BaseComponent implements OnInit {

    permissions = Permission;

    @ViewChild('header') headerComponent!: HeaderOperationsButtonsComponent;
    @ViewChild('popup') popupComponent!: PopupConfirmationComponent;
    @ViewChild('pop') popComponent!: PopupConfirmationComponent;
    /*
   ** Determina si esta en proceso de guardado
   */
    isSaving = signal<boolean>(false);
    isEditMode = signal<boolean>(false);
    isLoading = signal<boolean>(false);
    id = signal<number>(0);
    selectedFile = signal<File | null>(null);
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
            companyId: [{ value: null, disabled: true }, [Validators.required]],
            password: [{ value: null, disabled: true }, [Validators.required]],
        })
    }

    ngOnInit(): void {
        this.route.params.subscribe({
            next: (p) => {
                if (p['id']) {
                    this.form.controls['companyId'].setValue(p['id']);
                }
            },
            error: () => { }
        });

    }

    save(): void {
        if (this.isValidForm(this.form)) {
            const formData = new FormData();
  formData.append('companyId', this.form.controls['companyId'].value);
  formData.append('password', this.form.controls['password'].value);
  formData.append('certificateData', this.selectedFile()!);
            

            this.isSaving.set(true);
            this.service.saveCompanyCertificate(formData)
                .subscribe({
                    next: (r) => {
                        this.showNotificationSuccess(
                            'Guardado correcto', 'Certificado guardado'
                        );
                        this.isSaving.set(false);
                        this.router.navigate(['/home/companies'])
                    },
                    error: () => {
                        this.isSaving.set(false);
                        this.showMessageError('No se pudo editar el certificado');
                    }
                })
        }
    };

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

    onFileSelected(event: Event) {
        const input = event.target as HTMLInputElement;
        if (input.files && input.files.length > 0) {
            this.selectedFile.set(input.files[0]);
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