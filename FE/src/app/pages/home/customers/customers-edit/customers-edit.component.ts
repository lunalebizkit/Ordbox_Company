import { ElementRef, signal, OnInit, ViewChild, Component } from "@angular/core";
import { FormArray, FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { NzMessageService } from "ng-zorro-antd/message";
import { NzNotificationService } from "ng-zorro-antd/notification";
import { EntityService } from "../customer.service";
import { BaseComponent } from "../../../../common/components/base/base.component";
import { HeaderOperationsButtonsComponent } from "../../../../common/components/headers/buttons.oparations.header.component";
import { PopupConfirmationComponent } from "../../../../common/components/popup-confirmation/popup-confirmation.component";
import { NzSpinModule } from "ng-zorro-antd/spin";
import { NzCollapseModule } from "ng-zorro-antd/collapse";
import { NzFormModule } from "ng-zorro-antd/form";
import { NzLayoutModule } from "ng-zorro-antd/layout";
import { ActivatedRoute, Router } from "@angular/router";
import { NzInputModule } from "ng-zorro-antd/input";
import { NzIconModule } from "ng-zorro-antd/icon";
import { NzButtonModule } from "ng-zorro-antd/button";
import { Permission } from "../../../../common/auth/models/permissions.enum";


@Component({
    selector: 'app-customers.edit',
    templateUrl: './customers-edit.component.html',
    imports: [HeaderOperationsButtonsComponent, NzSpinModule, NzCollapseModule, NzFormModule, NzLayoutModule, ReactiveFormsModule, PopupConfirmationComponent, NzInputModule, NzIconModule, NzButtonModule]
})

export class CustomersEditComponent extends BaseComponent implements OnInit {

    @ViewChild('header') headerComponent!: HeaderOperationsButtonsComponent;
    @ViewChild('popup') popupComponent!: PopupConfirmationComponent;
    @ViewChild('pop') popComponent!: PopupConfirmationComponent;
    /*
   ** Determina si esta en proceso de guardado
   */
    isSaving = signal<boolean>(false);
    isEditMode= signal<boolean>(false);
    permissions = Permission;
    /*
     ** Determina si esta buscando el usuario
     */
    isLoading = signal<boolean>(false);

    /*
     ** id del usuario a editar, si es nuevo...
     */
    id = signal<number>(0);
    /*
** Formulario
*/
    form!: FormGroup;
    get phoneNumberArray() {
        return this.form.controls['phoneEntity'] as FormArray;
    };

    get phoneNumberControls() {
        return this.phoneNumberArray.controls as FormControl[]
    };

    get emailsArray() {
        return this.form.controls['emailEntity'] as FormArray;
    };

    get emailsControls() {
        return this.emailsArray.controls as FormControl[]
    };

    constructor(
        private service: EntityService,
        notificacionService: NzNotificationService,
        el: ElementRef,
        message: NzMessageService,
        private fb: FormBuilder,
        private route: ActivatedRoute,
        private router: Router,
    ) {
        super(notificacionService, el, message);
        this.form = this.fb.group({
            dni: [{ value: '',disabled: true}, [Validators.required, Validators.pattern, Validators.maxLength]],
            cuit: [{ value: '',disabled: true}, [Validators.maxLength, Validators.pattern]],
            name: [{ value: '',disabled: true},[Validators.required]],
            address: [{ value: '',disabled: true}, [Validators.required]],
            observation: [{ value: '',disabled: true}],
            phoneEntity: new FormArray([]),
            emailEntity: new FormArray([])
        })
    }

    ngOnInit(): void {
        this.route.params.subscribe({
            next: (p) => {
                if (p['id']) {
                    this.isLoading.set(true)
                    this.getEntity(p['id']);
                }
            },
            error: () => { }
        })
    }

    getEntity(id: number): void {
        if (id != 0) {
            this.id.set(id);
            this.service.getById(id).subscribe({
                next: (r) => {
                    this.form.controls['dni'].setValue(r.dni);
                    this.form.controls['cuit'].setValue(r.cuit);
                    this.form.controls['name'].setValue(r.name);
                    this.form.controls['address'].setValue(r.address);
                    this.form.controls['observation'].setValue(r.observation);
                    r.phoneEntity.forEach((e: any) => {
                        this.phoneNumberArray.push(new FormControl({ value: e,disabled: true}, [Validators.required]));
                    });
                    r.emailEntity.forEach((e: any) => {
                        this.emailsArray.push(new FormControl({value:`${e}`,disabled: true}, [Validators.required]));
                    });
                    this.isLoading.set(false);
                },
                error: (r) => {
                    this.isLoading.set(false);
                    this.showMessageError(r.error.descripcion);
                }
            })
        }
    };

    save(): void {
        if (!this.isValidForm(this.form)) return;
        const model = this.form.getRawValue();
        model.id = this.id;
        this.isSaving.set(true);
        this.service.saveCustomer(model).subscribe({
            next: (r) => {
                this.showNotificationSuccess(
                    'Guardado correcto',
                    `Se guardo correctamente el Cliente ${model.name}`
                );
                this.isSaving.set(false);
                this.popComponent.handleCance();
                this.router.navigate(['home/customers']);
            },
            error: () => {
                this.isSaving.set(false);
                this.showMessageError('No se pudo Guardar el Cliente');
            }
        })
    };

    addPhoneField(e?: MouseEvent): void {
        if (e) {
            e.preventDefault();
        }

        let phoneNumberForm = this.form.controls['phoneEntity'] as FormArray;
        phoneNumberForm.push(new FormControl(''));
    };

    addEmailField(e?: MouseEvent): void {
        if (e) {
            e.preventDefault();
        }
        let emailForm = this.form.controls['emailEntity'] as FormArray;
        emailForm.push(new FormControl('', Validators.email));
    };

    removeEmailField(e: MouseEvent, index: number): void {
        e.preventDefault();
        this.emailsArray.removeAt(index);
    };

    removePhoneField(e: MouseEvent, index: number): void {
        e.preventDefault();
        this.phoneNumberArray.removeAt(index);
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

    toggleEdit() {
        this.isEditMode.set(!this.isEditMode());
        if (this.isEditMode()) {
            this.form.enable();
        } else {
            this.form.disable();
        }
    }

}