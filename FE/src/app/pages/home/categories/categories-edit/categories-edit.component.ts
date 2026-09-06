import { ElementRef, Input, OnInit, ViewChild, Component, signal } from "@angular/core";
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { NzMessageService } from "ng-zorro-antd/message";
import { NzNotificationService } from "ng-zorro-antd/notification";
import { CategoriesService } from "../category.services";
import { CategoryModel } from "../model/category.model";
import { BaseComponent } from "../../../../common/components/base/base.component";
import { HeaderOperationsButtonsComponent } from "../../../../common/components/headers/buttons.oparations.header.component";
import { PopupConfirmationComponent } from "../../../../common/components/popup-confirmation/popup-confirmation.component";
import { NzCollapseModule } from "ng-zorro-antd/collapse";
import { NzFormModule } from "ng-zorro-antd/form";
import { NzSpinModule } from "ng-zorro-antd/spin";
import { ActivatedRoute } from "@angular/router";
import { NzInputModule } from "ng-zorro-antd/input";
import { Permission } from "../../../../common/auth/models/permissions.enum";


@Component({
  selector: 'app-categories-edit',
  templateUrl: './categories-edit.component.html',
  imports: [PopupConfirmationComponent, NzCollapseModule, NzFormModule, NzSpinModule, HeaderOperationsButtonsComponent, ReactiveFormsModule, NzInputModule]
})

export class CategoryEditComponent extends BaseComponent implements OnInit {

  /*
** Header
*/
  @ViewChild('header') headerComponent!: HeaderOperationsButtonsComponent;
  @ViewChild('popup') popupComponent!: PopupConfirmationComponent;
  @ViewChild('pop') popComponent!: PopupConfirmationComponent;

  /*
 ** Determina si esta en proceso de guardado
 */
  isSaving = signal<boolean>(false);
  permissions = Permission;
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
** Formulario
*/
  form!: FormGroup;

  constructor(
    private service: CategoriesService,
    notificacionService: NzNotificationService,
    el: ElementRef,
    message: NzMessageService,
    private fb: FormBuilder,
    private route: ActivatedRoute,
  ) {
    super(notificacionService, el, message);
    this.form = this.fb.group({
      description: [{ value: '', disabled: true }, [Validators.required]],
    })
  }

  ngOnInit(): void {
    this.route.params.subscribe({
      next: (p) => {
        if (p['id']) {
          this.isLoading.set(true);
          this.getCategory(p['id']);
        }
      },
      error: () => { }
    });
  }

  getCategory(id: number): void {
    if (id != 0) {
      this.id.set(id);
      this.isLoading.set(true);
      this.service.getCategoryById(id).subscribe({
        next: (r) => {
          this.form.controls['description'].setValue(r.description);
          this.isLoading.set(false);
        },
        error: () => { this.isLoading.set(false); }
      })
    }
  }

  save(): void {
    if (this.isValidForm(this.form)) {
      const model: CategoryModel = {
        id: this.id(),
        description: this.form.controls['description'].value,
      };
      this.isSaving.set(true);
      this.service.saveCategory(model)
        .subscribe({
          next: (r) => {
            this.showNotificationSuccess(
              'Guardado correcto',
              `Se guardo correctamente la Categoria ${model.description}`
            );
            this.isSaving.set(false);
          },
          error: () => {
            this.isSaving.set(false);
            this.showMessageError('No se pudo Guardar la Categoria');
          }
        })
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