import { Component, ElementRef, Input, OnInit, signal, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { BrandsService } from '../brands.services';
import { BrandsModel } from '../model/brands.model';
import { BaseComponent } from '../../../../common/components/base/base.component';
import { HeaderOperationsButtonsComponent } from '../../../../common/components/headers/buttons.oparations.header.component';
import { PopupConfirmationComponent } from '../../../../common/components/popup-confirmation/popup-confirmation.component';
import { NzSpinModule } from 'ng-zorro-antd/spin';
import { NzCollapseModule } from 'ng-zorro-antd/collapse';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { Permission } from '../../../../common/auth/models/permissions.enum';

@Component({
  selector: 'app-brands-edit',
  templateUrl: './brands-edit.component.html',
  imports: [HeaderOperationsButtonsComponent, NzSpinModule, NzCollapseModule, NzFormModule, ReactiveFormsModule, PopupConfirmationComponent, NzFormModule, NzInputModule]
})
export class BrandsEditComponent extends BaseComponent implements OnInit {

  permissions = Permission;
  @ViewChild('header') headerComponent!: HeaderOperationsButtonsComponent;
  @ViewChild('popup') popupComponent!: PopupConfirmationComponent;
  @ViewChild('pop') popComponent!: PopupConfirmationComponent;

  isLoading = signal<boolean>(false);
  isSaving = signal<boolean>(false);
  id = signal<number>(0);
  isEditMode= signal<boolean>(false);
  form!: FormGroup;

  constructor(
    private service: BrandsService,
    notificacionService: NzNotificationService,
    el: ElementRef,
    message: NzMessageService,
    private route: ActivatedRoute,
    private fb: FormBuilder,
  ) {
    super(notificacionService, el, message);
    this.form = this.fb.group({
      description: [{ value: '', disabled: true }, Validators.required],
    });
  }

  ngOnInit(): void {
    this.route.params.subscribe({
      next: (p) => {
        if (p['id']) {
          this.isLoading.set(true);
          this.getBrand(p['id']);
          this.id.set(p['id']);
        }
      },
      error: () => { }
    });
  }

  getBrand(id: number): void {
    if (id != 0) {
      this.service.getById(id).subscribe({
        next: (r) => {
          this.form.controls['description'].setValue(r.description);
          this.isLoading.set(false);
        },
        error: () => {
          this.isLoading.set(false);
        },
      });
    }
  }

  save(): void {
    if (this.isValidForm(this.form)) {
      const model: BrandsModel = {
        id: this.id() !== undefined ? this.id() : 0,
        description: this.form.controls['description'].value,
      };
      this.isSaving.set(true);
      this.service.saveBrand(model).subscribe({
        next: (r) => {
          this.showNotificationSuccess(
            'Guardado correcto',
            `Se guardo correctamente la Marca ${model.description}`
          );

          this.isSaving.set(false);
        },
        error: () => {
          this.isSaving.set(false);
          this.showMessageError('No se pudo Guardar la Marca');
        },
      });
    }
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
