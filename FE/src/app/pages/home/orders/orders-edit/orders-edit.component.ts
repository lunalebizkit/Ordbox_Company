import { formatCurrency } from '@angular/common';
import {
  Component,
  computed,
  ElementRef,
  Inject,
  LOCALE_ID,
  OnInit,
  signal,
  ViewChild,
} from '@angular/core';
import {
  FormArray,
  FormBuilder,
  FormControl,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { EntityService } from '../../customers/customer.service';
import { CustomerModel } from '../../customers/model/customer.model';
import { ProductsModel } from '../../products/model/product.model';
import { ProductService } from '../../products/product.service';
import {
  NewOrder,
  OrderDetailGrid,
  orderGridParser,
  orderGridProductParser,
  NewOrderDetail,
  orderNewProductParser,
  orderOldProductParser,
} from '../models/order.model';
import { OrdersService } from '../orders.service';
import { eStatus } from '../models/status-type.enum';
import { SendOrderEmail } from '../models/sendorderemail.model';
import { BaseComponent } from '../../../../common/components/base/base.component';
import { HeaderOperationsButtonsComponent } from '../../../../common/components/headers/buttons.oparations.header.component';
import { PopupConfirmationComponent } from '../../../../common/components/popup-confirmation/popup-confirmation.component';
import { NzCollapseModule } from 'ng-zorro-antd/collapse';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzLayoutModule } from 'ng-zorro-antd/layout';
import { FormatDatePipe } from '../../../../common/pipes/date.pipe';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzSwitchModule } from 'ng-zorro-antd/switch';
import { NzDividerModule } from 'ng-zorro-antd/divider';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzTableModule } from 'ng-zorro-antd/table';
import { NzInputNumberModule } from 'ng-zorro-antd/input-number';
import { NzSpaceModule } from 'ng-zorro-antd/space';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { Permission } from '../../../../common/auth/models/permissions.enum';
import { ActivatedRoute } from '@angular/router';
import { ProductFilter, resetProductFilter } from '../../../../common/components/model/product.filter.model';
import { InvoiceProductSearchComponent } from '../../invoices/invoice-product-search/invoice-product-search.component';
import { NzDrawerModule, NzDrawerService } from 'ng-zorro-antd/drawer';

@Component({
  selector: 'app-orders-edit',
  templateUrl: './orders-edit.component.html',
  styleUrls: ['./orders-edit.component.css'],
  imports: [HeaderOperationsButtonsComponent, PopupConfirmationComponent, NzCollapseModule, ReactiveFormsModule, NzFormModule, NzLayoutModule, FormatDatePipe, NzSelectModule, NzSwitchModule, NzDividerModule, NzInputModule, NzTableModule, NzInputNumberModule, NzSpaceModule, NzIconModule, FormsModule, NzButtonModule, NzDrawerModule]
})
export class OrdersEditComponent extends BaseComponent implements OnInit {

  @ViewChild('header') headerComponent!: HeaderOperationsButtonsComponent;
  @ViewChild('popup') popupComponent!: PopupConfirmationComponent;
  @ViewChild('pop') popComponent!: PopupConfirmationComponent;

  /*
   ** Formularios
   */
  isEditMode = signal<boolean>(false);
  isInvoiceEditable = signal<boolean>(true);
  permissions = Permission;
  form!: FormGroup;
  formProductSearch!: FormGroup;
  formSupplierSearch!: FormGroup;
  /*
   ** Sppiner
   */

  isSaving = signal<boolean>(false);
  loading = signal<boolean>(false);
  isLoading = signal<boolean>(false);

  /*
   ** Switch
   */
  switchValue = signal<boolean>(false);
  switchSendValue = signal<boolean>(false);
  emailList = signal<string[]>([]);

  /*
   ** Variables globales
   */
  id = signal<number>(0);
  product= signal<string>('');
  today = new Date();
  newOrder = signal<boolean>(true);
  paymentSelected: any;
  supplierName = signal<string>('');
  supplierEmail!: string;
  statusId = signal<number>(1);
  allStatus: { value: number; label: string }[] = Object.entries(eStatus)
    .filter(([key, value]) => typeof value === 'number')
    .map(([key, value]) => ({ value: value as number, label: key }));
  status: number = 0;
  email!: string;

  /*
   ** Fecha
   */
  dateTime!: Date;
  date = Date.now();

  /*
   ** Si algunos campos son visibles o no
   */
  /**Estado Rechazado y Aceptado (1) */
  viewOrder = signal<boolean>(true);
  /**Estado Pendiente (1) */
  editOrder = signal<boolean>(false);
  disableMail = signal<boolean>(false);

  /*
   ** Deshabilitar
   */
  disabled = signal<boolean>(false);

  /*
   ** Lista de Detalle Productos/Orders7Costumer
   */
  orderDetailGrid = signal<OrderDetailGrid[]>([]);
  orderDetail = signal<NewOrderDetail[]>([]);
  entityList: CustomerModel[] = [];
  /*
   ** Cantidad total de productos
   */
  totalItems: number = 0;

  subtotal = computed(() => this.orderDetailGrid().reduce((acc, item) => acc + item.price * item.orderedQuantity, 0));
  iva: number = 21;
  total = computed(() =>
    this.orderDetailGrid().reduce((acc, item) => acc + item.subTotal, 0));

  ivaTotal: number = 0;

  /*
   **Variables de la tabla detalle
   */
  editId = signal<number | null>(null);
  editIdrecievedQuantity: number | null = null;

  /*
   ** Parametros de busqueda
   */
  queryParams = {
    filter: '',
    page: 0,
    pageSize: 20,
  };

  queryData: ProductFilter = resetProductFilter;

  constructor(
    notificacionService: NzNotificationService,
    el: ElementRef,
    message: NzMessageService,
    private fb: FormBuilder,
    private entityService: EntityService,
    private ordersService: OrdersService,
    private serviceProduct: ProductService,
    private route: ActivatedRoute,
    @Inject(LOCALE_ID) public locale: string,
    private drawerService: NzDrawerService,
  ) {
    super(notificacionService, el, message);
    this.form = this.fb.group({
      statusId: [1, [Validators.required]],
      isPaid: [false],
      datetime: [new Date(), [Validators.required]],
      supplierEmail: new FormArray([]),
      emailEntity: new FormArray([]),
      observation: [{ value: '', disabled: false }]
    });
    this.formProductSearch = this.fb.group({
      productSearchFilter: ['', [Validators.required]],
    });
    this.formSupplierSearch = this.fb.group({
      supplierId: ['', [Validators.required]],
    });
  }

  ngOnInit(): void {
    this.route.params.subscribe({
      next: (p) => {
        if (p['id']) {
          this.isLoading.set(true);
          this.getOrder(p['id']);
          this.id.set(p['id']);
        }
      },
      error: () => { }
    });
  }

  /*
   ** Busqueda Proveedor
   */
  onSearch(data: string): void {
    if (data.length > 2) {
      this.queryParams.page = 0;
      this.queryParams.filter = data;
      this.getSupplier(this.queryParams);
    }
  }

  getSupplier(params: any): void {
    this.loading.set(true);
    this.entityService.getSuppliers(params).subscribe({
      next: (r) => {
        this.entityList = r.data;
        this.totalItems = r.totalCount;
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.entityList = [];
      },
    });
  }

  onChange(id: number) {
    if (id != 0 && id != null)
      this.entityService.getSupplierById(id).subscribe({
        next: (r) => {
          r.emailEntity.forEach((e: any) => {
            this.emailsEntityArray.push(
              new FormControl({ value: `${e}`, disabled: true }, [Validators.required])
            );
          });
        },
        error: () => { },
      });
  }

  /*
   ** Obtener orden
   */

  getOrder(id: number): void {
    if (id != 0) {
      this.ordersService.getById(id).subscribe({
        next: (r) => {
          this.newOrder.set(false);
          this.form.patchValue({
            statusId: r.statusId,
            isPaid: r.isPaid,
            observation: r.observation,
          });
          this.supplierName.set(r.supplierName);
          this.formSupplierSearch.controls['supplierId'].setValue(r.supplierId);
          this.dateTime = r.dateTime;
          r.supplierEmail.forEach((e: any) => {
            this.emailsArray.push(
              new FormControl({ value: `${e}`, disabled: true }, [Validators.required])
            );
          });
          // /*Bindeo detalles*/
          // r.orderDetail.forEach((orderDetail: OrderDetailGrid) => {
          //   /**Parseo viejo Producto a Grid */
          //   this.orderListGridTest.push(orderGridParser(orderDetail));
          this.orderDetailGrid.set(r.orderDetail.map((item: OrderDetailGrid) => orderGridParser(item)));
          this.orderDetail.set(r.orderDetail.map((item: OrderDetailGrid) => orderOldProductParser(item)));
          //   /* Parseo viejo Producto a Detalle*/
          //   this.orderDetail.push(orderOldProductParser(orderDetail));
          // });
          if (r.statusId == 1) {
            this.editOrder.set(true);
            this.disabled.set(false);
            this.disableMail.set(true);
            this.viewOrder.set(false);
          } else {
            this.viewOrder.set(true);
            this.editOrder.set(false);
            this.disabled.set(true);
            this.disableMail.set(false);
          }

          this.isLoading.set(false);
        },
        error: () => {
          this.isLoading.set(false);
        },
      });
    }
  }

  /*
   ** Guardar o actualizar una orden
   */

  save(): void {
    if (
      this.isValidForm(this.form) &&
      this.isValidForm(this.formSupplierSearch)
    ) {
      if (this.orderDetail.length === 0) {
        this.showMessageError('No hay Productos Seleccionados');
      } else {
        const model: NewOrder = {
          id: this.id() !== undefined ? this.id() : 0,
          supplierId: this.formSupplierSearch.controls['supplierId'].value,
          isPaid: this.form.controls['isPaid'].value,
          statusId:
            this.id() != undefined && this.id() == 0
              ? 1
              : this.form.controls['statusId'].value,
          orderDetail: this.orderDetail(),
          dateTime: this.form.controls['datetime'].value,
          supplierName: null,
          supplierEmail: this.form.controls['emailEntity'].value,
          observation: this.form.controls['observation'].value
        };
        this.isSaving.set(true);
        this.ordersService.saveOrder(model).subscribe({
          next: (r) => {
            this.showNotificationSuccess(
              'Guardado correcto',
              `Se guardo correctamente el pedido`
            );
            this.isSaving.set(false);

          },
          error: () => {
            this.isSaving.set(false);
            this.showMessageError('No se pudo Guardar el pedido');
          },
        });
      }
    }
  }

  /*
   ** Obtener emails de proveedores
   */

  get emailsArray() {
    return this.form.controls['supplierEmail'] as FormArray;
  }
  get emailsControls() {
    return this.emailsArray.controls as FormControl[];
  }

  get emailsEntityArray() {
    return this.form.controls['emailEntity'] as FormArray;
  }

  get emailsEntityControls() {
    return this.emailsEntityArray.controls as FormControl[];
  }

  startEdit(id: number): void {
    this.editId.set(id);
  }

  stopEdit(): void {
    this.editId.set(null);
  }

  /*
   ** Cambiar cantidades el pedido
   */

  changeQuantity(quantity: number): void {
    if (!quantity || quantity <= 0) quantity = 1;

    this.orderDetailGrid.update(item =>
      item.map(detail => detail.id === this.editId()
        ? { ...detail, quantity, subTotal: quantity * detail.price }
        : detail
      ));

    this.orderDetail.update(item =>
      item.map(detail => detail.productId === this.editId()
        ? { ...detail, orderedQuantity: quantity }
        : detail
      ));
  }

  startEditrecievedQuantity(id: number): void {
    this.editIdrecievedQuantity = id;
  }

  stopEditrecievedQuantity(): void {
    this.editIdrecievedQuantity = null;
  }

  changeQuantityrecievedQuantity(quantity: number): void {
    if (quantity == 0 || quantity == null) {
      quantity = 0;
    }

    this.orderDetail.update(item => item.map(detail =>
      detail.productId === this.editIdrecievedQuantity
        ? { ...detail, recievedQuantity: quantity } : detail
    ));
  }

  /*
   ** Busqueda Producto
   */

  searchProduct(): void {
    this.product.set(this.formProductSearch.controls['productSearchFilter'].value);

    this.queryData.filter.product = this.product();

    if (this.product().length > 0) {
      this.serviceProduct.getProducts(this.queryData).subscribe({
        next: (r) => {
          if (r.data.length == 1) {
            const product: ProductsModel = r.data[0];
            this.addOrUpdateProduct(product);
          }
          else {
            this.openComponentProduct();
          }
        },
        error: () => { },
      });
    }
  }

  handleOk() {
    try {
      const idToDelete = this.popupComponent.elementSelectedToDelete();
      const updatedList = this.orderDetail().filter(el => el.productId !== idToDelete);
      const updatedGrid = this.orderDetailGrid().filter(el => el.id !== idToDelete);
      this.orderDetail.set(updatedList);
      this.orderDetailGrid.set(updatedGrid);

      this.popupComponent.isDeleteConfirmationVisible.set(false);

    } catch (error) {
      console.log(error);
    }
  }


  msjConfirmOk() {
    try {

      const idToDelete = this.popupComponent.elementSelected();

      const updatedList = this.orderDetail().filter(el => el.productId !== idToDelete);
      this.orderDetail.set(updatedList);

      this.popupComponent.isConfirmationvisible.set(false);
      if (
        this.isValidForm(this.form) && this.isValidForm(this.formSupplierSearch) &&
        this.orderDetail().length != 0) {
        this.popComponent.showConfirmation()
      } else {
        this.showMessageError('No ha seleccionado producto');
      }
    } catch (error) {
      console.log(error);

    }
  }

  msjConfirmOkEmail() {
    try {
      const idToDelete = this.popupComponent.elementSelected();
      const updatedList = this.orderDetail().filter(el => el.productId !== idToDelete);
      this.orderDetail.set(updatedList);

      this.popupComponent.isConfirmationvisible.set(false);
      if (
        this.isValidForm(this.form)
        || (this.orderDetailGrid().length === 0)) {
        this.saveAndSend();
      } else {
        this.showMessageError('No ha seleccionado producto');
      }
    } catch (error) {
      console.log(error);

    }
  }

  msjConfirmOkEmailOnly() {
    try {
      const idToDelete = this.popupComponent.elementSelected();
      const updatedList = this.orderDetail().filter(el => el.productId !== idToDelete);
      this.orderDetail.set(updatedList);

      this.popupComponent.isConfirmationvisible.set(false);
      if (
        this.isValidForm(this.form)
        || (this.orderDetailGrid().length === 0)) {
        this.sendEmail();
      } else {
        this.showMessageError('No ha seleccionado producto')
      }
    } catch (error) {
      console.log(error);

    }
  }

  getStatusName(id: number) {
    return eStatus[id];
  }

  currencyFormat(data: any): string {
    return formatCurrency(data, this.locale, '$', 'ARS', '1.1-2');
  }
  /*
     ** Enviar solo el email
     */
  sendEmail() {
    if (this.emailList().length > 0) {

      this.isSaving.set(true);

      const model: SendOrderEmail = {
        id: this.id(),
        emails: this.emailList()
      }
      this.ordersService.sendEmail(model).subscribe({
        next: (r) => {
          this.showNotificationSuccess(
            'Email enviado correctamente',
            `Se realizo correctamente el envio del email`
          );
          this.isSaving.set(false);
        },
        error: () => {
          this.isSaving.set(false);
          this.showMessageError('No se pudo realizar el envio del email');
        },
      })
    } else {
      this.showMessageError('No hay emails seleccionados');
    }
  }
  /*
   ** Guardar pedido y enviar email
   */
  saveAndSend(): void {
    if (this.emailList().length > 0) {
      if (
        this.isValidForm(this.form) &&
        this.isValidForm(this.formSupplierSearch)
      ) {
        if (this.orderDetail.length === 0) {
          this.showMessageError('No hay Productos Seleccionados');
        } else {
          this.isSaving.set(true);

          const model: NewOrder = {
            id: this.id() !== undefined ? this.id() : 0,
            supplierId: this.formSupplierSearch.controls['supplierId'].value,
            isPaid: this.form.controls['isPaid'].value,
            statusId:
              this.id() != undefined && this.id() == 0
                ? 1
                : this.form.controls['statusId'].value,
            orderDetail: this.orderDetail(),
            dateTime: this.form.controls['datetime'].value,
            supplierName: null,
            supplierEmail: this.emailList(),
            observation: this.form.controls['observation'].value
          };
          this.ordersService.saveOrderAndSendEmail(model).subscribe({
            next: (r) => {
              this.showNotificationSuccess(
                'Guardado y enviado correcto',
                `Se guardo correctamente el pedido y se envio el email`
              );
              this.isSaving.set(false);
            },
            error: () => {
              this.isSaving.set(false);
              this.showMessageError('No se pudo Guardar el pedido');
            },
          });
        }
      }
    } else {
      this.showMessageError('No hay emails seleccionados');
    }
  }
  /*
   ** Seleccionar email/s para enviar
   */

  selectEmails(data: string,) {

    let isEmail = this.emailList().find((email: string) => email == data);

    if (isEmail) {
      this.emailList.set(this.emailList().filter((email: string) => email != data));
    } else {
      this.emailList().push(data);
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

  addOrUpdateProduct(product: ProductsModel): void {
    const exists = this.orderDetail().some(item => item.productId === product.id);

  if (exists) {
    this.orderDetail.update(list =>
      list.map(item =>
        item.productId === product.id
          ? { ...item, orderedQuantity: item.orderedQuantity + 1 }
          : item
      )
    );

    this.orderDetailGrid.update(list =>
      list.map(item =>
        item.id === product.id
          ? {
              ...item,
              orderedQuantity: item.orderedQuantity + 1,
              subTotal: product.purchasePrice * (item.orderedQuantity + 1),
            }
          : item
      )
    );
    } else {
      // agregar nuevo producto
      const gridDetail: OrderDetailGrid = orderGridProductParser(product);
      const detail: NewOrderDetail = orderNewProductParser(product);

      this.orderDetailGrid.update(list => [...list, gridDetail]);
      this.orderDetail.update(list => [...list, detail]);
    }
  }

  openComponentProduct(): void {
    if (true) {
      const drawerRefProduct = this.drawerService.create<
        InvoiceProductSearchComponent,
        { filter: string; supplierId: number },
        [ProductsModel]
      >({
        nzTitle: 'Productos',
        nzContent: InvoiceProductSearchComponent,
        nzSize: 'large',
        nzWidth: '90%',
        nzContentParams: {
          filter: this.formProductSearch.controls['productSearchFilter'].value,
          supplierId: this.formSupplierSearch.controls['supplierId'].value
        },
        nzClosable: false,
      });

      drawerRefProduct.afterClose.subscribe({
        next: (data: [ProductsModel]| undefined) => {
          if (data != undefined) {

            data.forEach((productItem) => {
              this.addOrUpdateProduct(productItem);              
            })

          }
        },
        error: () => {
          this.orderDetailGrid.set([]);
        },
      });
    } else {
      return;
    }
  }
}
