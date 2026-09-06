import { Component, computed, ElementRef, Input, OnInit, signal, TemplateRef, ViewChild } from "@angular/core";
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from "@angular/forms";
import { NzMessageService } from "ng-zorro-antd/message";
import { NzNotificationService } from "ng-zorro-antd/notification";
import { NzDrawerModule, NzDrawerRef, NzDrawerService } from 'ng-zorro-antd/drawer';
import { EntityService } from "../../customers/customer.service";
import { ProductsModel } from "../../products/model/product.model";
import { ActivatedRoute, Router } from "@angular/router";
import { formatCurrency } from '@angular/common';
import { Inject, LOCALE_ID } from '@angular/core';
import { ProductService } from "../../products/product.service";
import { InvoiceProductSearchComponent } from "../../invoices/invoice-product-search/invoice-product-search.component";
import { CustomerAddModel } from "../../customers/model/customer.add.model";
import { pStatusType, statusType } from "../model/status.model";
import { isNil } from "ng-zorro-antd/core/util";
import { InvoiceCustomerSearchComponent } from "../../invoices/invoice-customer-search/invoice-customer-search.component";
import { BaseComponent } from "../../../../common/components/base/base.component";
import { PopupConfirmationComponent } from "../../../../common/components/popup-confirmation/popup-confirmation.component";
import { HeaderOperationsButtonsComponent } from "../../../../common/components/headers/buttons.oparations.header.component";
import { deliveryNotesDetailFromParser, deliveryNotesDetailParser, DeliveryNotesDetails, DeliveryNotesGrid, deliveryNotesGridFromParser, deliveryNotesGridParser, DeliveryNotesModel } from "../model/delivery-notes.model";
import { deliveryNotesService } from "../delivery-notes.service";
import { NzCollapseModule } from "ng-zorro-antd/collapse";
import { NzDividerModule } from "ng-zorro-antd/divider";
import { NzFormModule } from "ng-zorro-antd/form";
import { NzInputModule } from "ng-zorro-antd/input";
import { NzInputNumberModule } from "ng-zorro-antd/input-number";
import { NzLayoutModule } from "ng-zorro-antd/layout";
import { NzPageHeaderModule } from "ng-zorro-antd/page-header";
import { NzSelectModule } from "ng-zorro-antd/select";
import { NzSpaceModule } from "ng-zorro-antd/space";
import { NzSwitchModule } from "ng-zorro-antd/switch";
import { NzTableModule } from "ng-zorro-antd/table";
import { NzTagModule } from "ng-zorro-antd/tag";
import { AppCommonModule } from "../../../../common/app.common.module";
import { FormatDatePipe } from "../../../../common/pipes/date.pipe";
import { Permission } from "../../../../common/auth/models/permissions.enum";

@Component({
  selector: 'app-delivery-notes-edit',
  templateUrl: './delivery-notes-edit.component.html',
  styleUrls: ['./delivery-notes-edit.component.css'],
  imports: [HeaderOperationsButtonsComponent, FormsModule, NzFormModule, ReactiveFormsModule, NzCollapseModule, NzLayoutModule, FormatDatePipe, NzTagModule, NzPageHeaderModule, NzSelectModule, NzDividerModule, NzSwitchModule, AppCommonModule, NzTableModule, PopupConfirmationComponent, NzInputNumberModule, NzInputModule, NzSpaceModule, NzDrawerModule]
})
export class DeliveryNotesEditComponent extends BaseComponent implements OnInit {
  @ViewChild('popup') popupComponent!: PopupConfirmationComponent;
  @ViewChild('header') headerComponent!: HeaderOperationsButtonsComponent;
  @ViewChild('pop') popComponent!: PopupConfirmationComponent;

  @ViewChild('drawerTemplate', { static: false }) drawerTemplate?: TemplateRef<{
    $implicit: { filter: string },
    drawerRef: NzDrawerRef<string>;
  }>;
  tipo!: string;
  newStatusPaid!: number;
  paymentSelected: any;

  loading = signal<boolean>(false);
  isLoading = signal<boolean>(false);
  isSaving = signal<boolean>(false);

  id = signal<number>(0);
  editProductId: number = 0;
  editId = signal<number | null>(null);
  editIdIva = signal<number | null>(null);
  editIdProductPrice = signal<number | null>(null);
  editIdProductName = signal<number | null>(null);
  permissions = Permission;

  type = statusType;
  startDate = Date.now();
  form!: FormGroup;
  formProductSearch!: FormGroup;
  formProduct!: FormGroup;
  formSupplierSearch!: FormGroup;
  formDeliveryNotesModel!: FormGroup;

  dataDetails = signal<DeliveryNotesDetails[]>([]);
  dataGrid = signal<DeliveryNotesGrid[]>([]);
  isEditMode = signal<boolean>(false);
  isInvoiceEditable = signal<boolean>(false);

  cuit!: string;
  supplierId!: number;
  product= signal<string>('');
  pagado: boolean = true;
  typeSelectedId: number = 1;
  statusPaid!: boolean;
  totalItems: number = 0;
  userId!: number;

  name!: string;
  address!: string;
  editIdrecievedQuantity: number | null = null;
  productId!: number;
  deliveryNotesNumber!: number;
  observation!: string;
  dateTime!: Date;
  paid!: boolean;
  statusId!: number;

  subtotal = computed(() =>
    this.dataGrid().reduce((acc, item) =>
      acc + item.quantity * item.price, 0)
  );

  total = computed(() =>
    this.dataGrid().reduce((acc, item) =>
      acc + item.price * item.quantity, 0)
  );

  isDisabled = false;
  viewOrder: boolean = false;
  switchValue = false;
  isDisabledPaid = false;
  isDisabledGrabar = false;

  queryParams = {
    filter: '',
    page: 0,
    pageSize: 10,
  };

  constructor(notificacionService: NzNotificationService,
    private serviceEntity: EntityService,
    private serviceProduct: ProductService,
    private service: deliveryNotesService,
    private router: Router,
    private route: ActivatedRoute,
    el: ElementRef,
    message: NzMessageService,
    private drawerService: NzDrawerService,
    private fb: FormBuilder,
    @Inject(LOCALE_ID) public locale: string) {
    super(notificacionService, el, message)
    this.form = this.fb.group({
      dateTime: [new Date(this.startDate), Validators.required],
      statusId: [1, Validators.required],
      deliveryNotesNumber: [{ value: '', disabled: true }],
      supplierAddress: [{ value: '', disabled: true }, Validators.required],
      supplierCuit: [{ value: '', disabled: true }, [Validators.required, Validators.pattern('[0-9]{11}'),]],
      supplierDni: [{ value: '', disabled: true },],
      paid: [{ value: false, disabled: true }, Validators.required],
      supplierName: [{ value: '', disabled: true }, Validators.required],
      observation: [{ value: '', disabled: true }]
    });
    this.formSupplierSearch = this.fb.group({});
    this.formProductSearch = this.fb.group({
      productSearchFilter: [{ value: '', disabled: true }],
    });
  }


  ngOnInit() {

    this.route.params.subscribe({
      next: (p) => {
        if (p['id']) {
          this.getDeliveryNotes(p['id']);
        } else {
          this.isEditMode.set(true);
          this.isInvoiceEditable.set(true);
          this.form.enable();
          this.formProductSearch.enable();
        }
      },
      error: () => { }
    })
  }

  getDeliveryNotes(id: number): void {
    if (id != 0) {
      this.isLoading.set(true);
      this.id.set(id);
      this.service.getDeliveryNotesById(id).subscribe({
        next: (r) => {
          this.form.patchValue({
            dateTime: r.dateTime,
            statusId: r.statusId,
            deliveryNotesNumber: r.deliveryNotes_number,
            supplierAddress: r.supplierAddress,
            supplierCuit: r.supplierCuit,
            paid: r.paid,
            supplierName: r.supplierName,
            observation: r.observation
          });

          /*Bindeo detalles*/
          this.dataGrid.set(r.deliveryNotesDetails.map((modelDetail: DeliveryNotesDetails, index: number) => {
            return deliveryNotesGridFromParser(modelDetail, index)
          }));

          this.dataDetails.set(r.deliveryNotesDetails.map((modelDetail: DeliveryNotesDetails, index: number) => {
            return deliveryNotesDetailFromParser(modelDetail, index)
          }));


          if (r.paid === true) {
            this.isDisabledPaid = true;
          }

          if (r.statusId === 1 || r.statusId === 2) {
            this.isDisabled = true;
            this.isDisabledGrabar = true;
            this.form.controls['supplierAddress'].disable();
            this.form.controls['supplierCuit'].disable();
            this.form.controls['supplierName'].disable();
            this.form.controls['observation'].disable();
            this.viewOrder = true;
          } else {
            this.viewOrder = false;
          }

          if (this.isDisabled == true) {
            this.formProductSearch.controls['productSearchFilter'].disable();
          }
          this.isLoading.set(false);
        },
        error: () => {
          this.isLoading.set(false);
        },
      });
    }
  }

  getStatusName(id: number) { return pStatusType[id]; }

  searchSupplier(): void {
    this.cuit = this.form.controls['supplierCuit'].value;
    if (this.cuit === '00') {
      this.form.controls['supplierAddress'].setValue('S/D');
      this.form.controls['supplierCuit'].setValue('99999999995');
      this.form.controls['supplierName'].setValue('Admin');
      this.supplierId = 0;
      return;
    } else {
      if (this.cuit.length >= 6) {
        this.serviceEntity.getByCuit(this.cuit).subscribe({
          next: (data: any) => {
            this.form.controls['supplierAddress'].setValue(data.address);
            this.form.controls['supplierCuit'].setValue(data.cuit);
            this.form.controls['supplierName'].setValue(data.name);
          },
          error: () => {
            this.showMessageError('No se encontro Proveedor');
          },
        });
      }
    }
  }

  startEdit(id: number): void { this.editId.set(id); }

  stopEdit(): void { this.editId.set(null); }

  startEditProductPrice(id: number): void { this.editIdProductPrice.set(id); }

  stopEditProductPrice(): void { this.editIdProductPrice.set(null); }

  stopEditProductName(): void {
    this.editIdProductName.set(null);
  }

  startEditProductName(id: number): void {
    this.editIdProductName.set(id);
  }

  openComponentProduct(): void {
    const drawerRefProduct = this.drawerService.create<InvoiceProductSearchComponent, { filter: string }, [ProductsModel]>({
      nzTitle: 'Productos',
      nzContent: InvoiceProductSearchComponent,
      nzSize: 'large',
      nzWidth: '90%',
      nzContentParams: {
        filter: this.formProductSearch.controls['productSearchFilter'].value
      },
      nzClosable: false
    });
    drawerRefProduct.afterClose.subscribe({
      next: (data: [ProductsModel] | undefined) => {
        if (data != undefined) {
          data.forEach((productItem) => {
            this.addOrUpdateProduct(productItem);
          })
        }
      },
      error: () => {
        this.isLoading.set(false);
        this.formProductSearch.controls['productSearchFilter'].setValue('');
      }
    })
  }

  addOrUpdateProduct(product: ProductsModel): void {
    const existingDetail = this.dataDetails().find(item => item.productId === product.id);

    if (existingDetail) {
      // actualizar cantidad y subtotal
      existingDetail.quantity += 1;

      const gridDetail = this.dataGrid().find(item => item.ownCode === product.id);
      if (gridDetail) {
        gridDetail.quantity += 1;
        gridDetail.subTotal = product.cashSalePrice * product.quantity;
      }
    } else {
      // agregar nuevo producto
      const gridDetail: DeliveryNotesGrid = deliveryNotesGridParser(product, this.bindPrice(product));

      const detail: DeliveryNotesDetails = deliveryNotesDetailParser(product, this.bindPrice(product));

      this.dataGrid.update(list => [...list, gridDetail]);
      this.dataDetails.update(list => [...list, detail]);
    }
  }

  searchProduct(): void {
    this.product.set(this.formProductSearch.controls['productSearchFilter'].value);

    if (this.product() === '00') {
      this.addNewEditProduct();
      return;
    }

    if (!this.product() || this.product().length === 0) {
      this.isLoading.set(false);
      return;
    }

    const productParams = {
      filter: {
        product: this.product(),
        code: '',
        barCode: '',
        brand: 0,
        category: 0,
        status: 0,
        supplier: [] as Number[]
      },
      page: 0,
      pageSize: 50
    };

    if (this.product().length > 0) {
      this.serviceProduct.getProducts(productParams).subscribe({
        next: (r) => {
          this.isLoading.set(true);
          if (r.data.length === 1) {
            const product: ProductsModel = r.data[0];
            this.addOrUpdateProduct(product);
          }
          else {
            this.isLoading.set(false);
            this.openComponentProduct();
          }
          // if (r.data.length == 1) {
          //   const model: ProductsModel = r.data[0];
          //   if (this.deliveryNotesDetails.find(item => item.productId == model.id)) {
          //     /*Actualizo la lista que envio al back */
          //     this.deliveryNotesDetails.filter(item => item.productId == model.id)[0]
          //       .quantity += 1;

          //     /*Actualizo la lista de la tabla */
          //     this.deliveryNotesDetailsList.filter(item => item.ownCode == model.id)[0]
          //       .quantity += 1;

          //     this.deliveryNotesDetailsList.filter(item => item.ownCode == model.id)[0]
          //       .subTotal += this.bindPrice(model) * model.quantity;

          //     this.totalCalculate();
          //     this.isLoading = false;
          //     this.formProductSearch.controls['productSearchFilter'].setValue('');
          //   } else {
          //     const product: ProductsModel = r.data[0];
          //     // /* Parseo el Producto a la grilla de Tabla */
          //     const model: deliveryNotesDetailsList = deliveryNotesGridParser(product, this.bindPrice(product));

          //     this.deliveryNotesDetailsTest.push(model);
          //     this.deliveryNotesDetailsList = this.deliveryNotesDetailsTest;

          //     /* Parseo dato a Dto Factura Detalle */
          //     const modelDetail: DeliveryNotesDetails = deliveryNotesDetailParser(product, this.bindPrice(product));
          //     this.deliveryNotesDetails.push(modelDetail);
          //     this.totalCalculate();
          //     this.isLoading = false;
          //     this.formProductSearch.controls['productSearchFilter'].setValue('');
          //   }

          // } else {
          //   this.isLoading = false;
          //   this.openComponentProduct();
          // }

        },
        error: () => {
          this.isLoading.set(false);
          this.formProductSearch.controls['productSearchFilter'].setValue('');
        }
      })
    }
  };

  bindPrice(data: ProductsModel): number {
    const typePayment = this.paymentSelected;
    var a = Object.keys(data).filter(type => (type == typePayment));
    switch (a[0]) {

      default:
        return data.purchasePrice;
    }
  }

  currencyFormat(data: any): string {
    return formatCurrency(data, this.locale, '$', 'ARS', '1.1-2');
  }

  changeQuantity(quantity: number): void {
    if (!quantity || quantity <= 0) quantity = 1;

    this.dataGrid.update(list =>
      list.map(detail =>
        detail.ownCode === this.editId()
          ? { ...detail, quantity, subTotal: quantity * detail.price }
          : detail
      )
    );

    this.dataDetails.update(list =>
      list.map(detail =>
        detail.productId === this.editId()
          ? { ...detail, quantity }
          : detail
      )
    );
  }

  openComponentCustomer(): void {
    const drawerRefSupplier = this.drawerService.create<
      InvoiceCustomerSearchComponent,
      {},
      CustomerAddModel
    >({
      nzTitle: 'Cliente',
      nzContent: InvoiceCustomerSearchComponent,
      nzSize: 'large',
      nzWidth: '90%',
      nzClosable: false,
    });
    drawerRefSupplier.afterClose.subscribe({
      next: (data) => {
        if (data != undefined) {
          this.supplierId = data.id;
          this.form.controls['supplierAddress'].setValue(data.address);
          this.form.controls['supplierCuit'].setValue(!isNil(data.cuit) ? data.cuit.replace(/[^a-zA-Z0-9 ]/g, '') : null);
          this.form.controls['supplierName'].setValue(data.name);
          this.form.controls['supplierDni'].setValue(data.dni)
        }
      },
      error: () => { },
    });
  }

  typeSelectedChange(id: any): void {
    this.typeSelectedId = id;
    if (id == 1) {
      this.pagado = true;
    } else {
      this.pagado = false;
    }
  }

  save(): void {
    if (this.isValidForm(this.form)) {
      //EDITAR
      if (this.id() > 0) {
        const model: DeliveryNotesModel = {
          id: this.id(),
          supplierName: this.form.controls['supplierName'].value,
          supplierCuit: this.form.controls['supplierCuit'].value,
          supplierAddress: this.form.controls['supplierAddress'].value,
          observation: this.form.controls['observation'].value,
          paid: this.form.controls['paid'].value,
          statusId: this.form.controls['statusId'].value,
          importTotal: 0,
          dateTime: this.form.controls['dateTime'].value,
          deliveryNotesDetails: this.dataDetails(),
          deliveryNotes_number: this.id(),
          supplierId: this.supplierId,
          cancelled: ""
        };

        this.isSaving.set(true);
        this.service.editDeliveryNotes(model)
          .subscribe({
            next: (r) => {
              this.showNotificationSuccess(
                'Guardado correcto',
                `Remito editado correctamente`
              );
              this.isSaving.set(false);
              this.router.navigate(['/home/delivery-notes']);
            },
            error: (r) => {
              this.isSaving.set(false);
              this.showMessageError(r.error)
            }
          });

        //GUARDAR
      } else {
        const model: DeliveryNotesModel = {
          id: 0,
          supplierName: this.form.controls['supplierName'].value,
          supplierCuit: this.form.controls['supplierCuit'].value,
          supplierAddress: this.form.controls['supplierAddress'].value,
          observation: this.form.controls['observation'].value,
          paid: this.form.controls['paid'].value,
          statusId: this.form.controls['statusId'].value,
          importTotal: 0,
          dateTime: this.form.controls['dateTime'].value,
          deliveryNotesDetails: this.dataDetails(),
          deliveryNotes_number: this.id(),
          supplierId: this.supplierId,
          cancelled: ""
        };

        this.isSaving.set(true);
        this.service.saveDeliveryNotes(model)
          .subscribe({
            next: (r) => {
              this.showNotificationSuccess(
                'Guardado correcto',
                `Remito creado correctamente`
              );
              this.isSaving.set(false);
              this.router.navigate(['/home/delivery-notes']);
            },
            error: (r) => {
              this.isSaving.set(false);
              this.showMessageError(r.error)
            }
          });
      }
    }

  };


  handleOk() {
    try {
      const idToDelete = this.popupComponent.elementSelectedToDelete();

      const updatedGrid = this.dataGrid().filter(el => el.ownCode !== idToDelete);
      const updatedDetails = this.dataDetails().filter(el => el.productId !== idToDelete);

      this.dataGrid.set(updatedGrid);
      this.dataDetails.set(updatedDetails);

      this.popupComponent.isDeleteConfirmationVisible.set(false);
    } catch (error) { console.log(error); }
  }

  msjConfirmOk() {
    try {
      this.dataGrid.set(this.dataGrid().filter(element => element.ownCode != this.popupComponent.elementSelected()));

      this.popupComponent.isConfirmationvisible.set(false);

      if (this.dataGrid().length === 0) {
        this.showMessageError('No ha seleccionado producto');
        return;
      }

      if (this.isValidForm(this.formSupplierSearch) && this.isValidForm(this.formProductSearch) && this.isValidForm(this.form)) {
        this.popComponent.showConfirmation()
      }
    } catch (error) { console.log(error); }
  }

  selectPaid(value: boolean) {
    this.statusPaid = value;
  }

  addNewEditProduct(): void {
    this.editProductId--;

    let newEditProduct: ProductsModel = {
      id: this.editProductId,
      quantity: 1,
      code: '',
      description: ' ',
      cashSalePrice: 0,
      categoryName: '',
      brandName: '',
      purchasePrice: 0,
      salePrice: 0,
      salePercentage: 0,
      cardSalePrice: 0,
      cashSalePercentage: 0,
      cardSalePercentage: 0,
      pointOrder: 0,
      observation: '',
      supplierName: '',
      isDeleted: false,
      barCode: ''
    };

    /* Parseo el Producto a la grilla de Tabla */
    const model: DeliveryNotesGrid = deliveryNotesGridParser(newEditProduct, this.bindPrice(newEditProduct));
    this.dataGrid.update(item => [...item, model]);

    /* Parseo dato a Dto Factura Detalle */
    const modelDetail: DeliveryNotesDetails = deliveryNotesDetailParser(newEditProduct, this.bindPrice(newEditProduct));
    this.dataDetails.update(item => [...item, modelDetail]);

    this.isLoading.set(false);

    this.formProductSearch.controls['productSearchFilter'].setValue('');

  }

  changeProductName(name: string): void {
    this.dataGrid.update(list =>
      list.map(detail =>
        detail.ownCode === this.editIdProductName()
          ? { ...detail, productName: name }
          : detail
      )
    );

    this.dataDetails.update(list =>
      list.map(detail =>
        detail.productId === this.editIdProductName()
          ? { ...detail, productName: name }
          : detail
      )
    );
  }

  changeProductPrice(price: number): void {
    this.dataGrid.update(list =>
      list.map(detail =>
        detail.ownCode === this.editIdProductPrice()
          ? { ...detail, price, subTotal: detail.quantity * price }
          : detail
      )
    );

    this.dataDetails.update(list =>
      list.map(detail =>
        detail.productId === this.editIdProductPrice()
          ? { ...detail, price }
          : detail
      )
    );
  }

  toggleEdit() {
    this.isEditMode.set(!this.isEditMode());
    if (this.isEditMode()) {
      this.form.enable();
      this.formProductSearch.enable();
      this.isInvoiceEditable.set(true);
    } else {
      this.form.disable();
      this.formProductSearch.disable();
      this.isInvoiceEditable.set(false);
    }
  }
}
