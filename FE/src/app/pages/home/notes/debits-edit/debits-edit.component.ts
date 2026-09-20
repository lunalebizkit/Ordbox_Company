import { formatCurrency } from '@angular/common';
import { Component, computed, ElementRef, Inject, LOCALE_ID, OnInit, signal, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { NzDrawerModule, NzDrawerService } from 'ng-zorro-antd/drawer';

import { NzMessageService } from 'ng-zorro-antd/message';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { EntityService } from '../../customers/customer.service';
import { CustomerModel } from '../../customers/model/customer.model';
import { InvoiceProductSearchComponent } from '../../invoices/invoice-product-search/invoice-product-search.component';
import { InvoiceService } from '../../invoices/invoices.service';
import { ePayment } from '../../invoices/model/invoice-payment.Enum';
import { eInvoiceType, InvoiceType } from '../../invoices/model/invoice-type.Enum';
import { InvoiceDetails, InvoiceModel } from '../../invoices/model/invoice.model';
import { IvaType } from '../../invoices/model/iva-type.Enum';
import { ProductsModel } from '../../products/model/product.model';
import { ProductService } from '../../products/product.service';
import { debitMemoDetailFromInvoiceParser, debitMemoDetailParser, DebitMemoDetails, DebitMemoGrid, debitMemoGridFromInvoiceParser, debitMemoGridParser, DebitMemoModel } from '../model/debitMemo.model';
import { NoteService } from '../notes.service';
import { BaseComponent } from '../../../../common/components/base/base.component';
import { PopupConfirmationComponent } from '../../../../common/components/popup-confirmation/popup-confirmation.component';
import { AuthService } from '../../../../common/auth/interceptors/auth.service';
import { Permission } from '../../../../common/auth/models/permissions.enum';
import { InvoiceVersion } from '../../../../common/auth/models/invoice-versions.enum';
import { HeaderOperationsButtonsComponent } from '../../../../common/components/headers/buttons.oparations.header.component';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzCollapseModule } from 'ng-zorro-antd/collapse';
import { NzLayoutModule } from 'ng-zorro-antd/layout';
import { FormatDatePipe } from '../../../../common/pipes/date.pipe';
import { NzTagModule } from 'ng-zorro-antd/tag';
import { NzPageHeaderModule } from 'ng-zorro-antd/page-header';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzDividerModule } from 'ng-zorro-antd/divider';
import { NzSwitchModule } from 'ng-zorro-antd/switch';
import { NzInputModule } from 'ng-zorro-antd/input';
import { AppCommonModule } from "../../../../common/app.common.module";
import { NzTableModule } from 'ng-zorro-antd/table';
import { NzInputNumberModule } from 'ng-zorro-antd/input-number';
import { NzSpaceModule } from 'ng-zorro-antd/space';
import { InvoiceLog } from '../../invoices/model/invoice-log-integration';
import { XMLParser } from "fast-xml-parser";
import { InvoiceCustomerSearchComponent } from '../../invoices/invoice-customer-search/invoice-customer-search.component';
import { isNil } from 'ng-zorro-antd/core/util';

@Component({
  selector: 'app-debits-edit',
  templateUrl: './debits-edit.component.html',
  styleUrls: ['./debits-edit.component.css'],
  imports: [HeaderOperationsButtonsComponent, FormsModule, NzFormModule, ReactiveFormsModule, NzCollapseModule, NzLayoutModule, FormatDatePipe, NzTagModule, NzPageHeaderModule, NzSelectModule, NzDividerModule, NzSwitchModule, AppCommonModule, NzTableModule, PopupConfirmationComponent, NzInputNumberModule, NzInputModule, NzSpaceModule, NzDrawerModule]
})
export class DebitsEditComponent extends BaseComponent implements OnInit {
  @ViewChild('popup') popupComponent!: PopupConfirmationComponent;
  @ViewChild('pop') popComponent!: PopupConfirmationComponent;

  cuit!: string;
  startDate = Date.now();
  stock!: number;
  ownCode!: number;
  code!: number;
  productName!: string;
  quantity!: number;
  price!: number;
  editProductId: number = 0;
  editId = signal<number | null>(null);
  editIdIva = signal<number | null>(null);
  editIdProductPrice = signal<number | null>(null);
  editIdProductName = signal<number | null>(null);
  iva: number = 21;
  debitId!: number;

  dateTime!: Date;

  dataDetails = signal<DebitMemoDetails[]>([]);
  dataGrid = signal<DebitMemoGrid[]>([]);
  cae = signal<string | null>(null);
  integrationSuccess = signal<boolean | null>(null);
  customerId = signal<number>(0);

  formDebitMemo: FormGroup;
  formCustomerSearch!: FormGroup;
  formProductSearch: FormGroup;
  product: any;
  queryParams = {
    filter: '',
    page: 0,
    pageSize: 10
  };
  paymentSelected: any;
  payment: { value: string; label: string }[] = Object.entries(ePayment).map(([value, label]) => ({ value, label }))
  invoiceA: boolean = true;
  type = InvoiceType;
  type1!: number
  ivaType = IvaType;
  typeSelectedId!: number;
  ivaSelectedId: number = 1;
  ivaSelected!: number;
  selectedDni: boolean = false;
  dni: any;

  loading = signal<boolean>(false);
  isLoading = signal<boolean>(false);
  isSaving = signal<boolean>(false);
  isEditMode = signal<boolean>(false);
  isInvoiceEditable = signal<boolean>(false);
  logLoading = signal<boolean>(false);
  permissions = Permission;
  id = signal<number | null>(null);
  userId = signal<number>(0);
  invoiceLog = signal<InvoiceLog[]>([]);
  version = signal<InvoiceVersion>(0);
  invoiceVersion = InvoiceVersion;

  subtotal = computed(() =>
    this.dataGrid().reduce((acc, item) =>
      acc + (item.quantity * this.ivaCalculate(item.price, item.iva)), 0)
  );

  ivaTotal = computed(() =>
    this.dataGrid().reduce((acc, item) =>
      acc + (item.price - this.ivaCalculate(item.price, item.iva)) * item.quantity, 0)
  );

  total = computed(() =>
    this.dataGrid().reduce((acc, item) =>
      acc + item.price * item.quantity, 0)
  );

  constructor(@Inject(LOCALE_ID) public locale: string,
    private serviceInvoice: InvoiceService,
    private serviceProduct: ProductService,
    public service: NoteService,
    private serviceEntity: EntityService,
    public serviceUser: AuthService,
    private fb: FormBuilder,
    private router: Router,
    private route: ActivatedRoute,
    private drawerService: NzDrawerService,
    notificacionService: NzNotificationService,
    el: ElementRef,
    message: NzMessageService,

  ) {
    super(notificacionService, el, message);
    this.formDebitMemo = this.fb.group({
      dateTime: [{ value: new Date(this.startDate), disabled: true }, Validators.required],
      type: [{ value: 1, disabled: true }, Validators.required],
      customerAddress: [{ value: '', disabled: true }, Validators.required],
      customerCuit: [{ value: '', disabled: true }, [Validators.required, Validators.pattern('[0-9]{8,11}'), Validators.minLength]],
      customerName: [{ value: '', disabled: true }, Validators.required],
      customerDni: [{ value: '', disabled: true }],
      invoiceNumber: [{ value: 0, disabled: true }, Validators.required],
      observation: [{ value: '', disabled: true }]
    });
    this.formCustomerSearch = this.fb.group({})
    this.formProductSearch = this.fb.group({
      productSearchFilter: [{ value: '', disabled: true }]
    })
  }

  ngOnInit(): void {
    this.route.params.subscribe({
      next: (p) => {
        const id = Number(p['id']);
        const source = history.state.source;
        if (id && source) {
          this.switchGetData(id, source);
        } else {
          this.isEditMode.set(true);
          this.isInvoiceEditable.set(true);
          this.formDebitMemo.enable();
          this.formProductSearch.enable();
          this.userId.set(this.serviceUser.currentUser()?.id ?? 0);
        }
      },
      error: () => {
        this.id.set(null);
      }
    })
  }

  switchGetData(id: number, source: string): void {
    this.isLoading.set(true);

    switch (source) {
      case 'invoices':
        this.getInvoice(id);
        break;
      case 'debits':
        this.getDebitMemo(id);
        break;

      default:
        this.isLoading.set(false);
        break;
    }
  }

  getInvoice(id: number): void {
    if (id != 0 || id !== undefined) {
      this.isLoading.set(true);
      this.id.set(id);
      this.serviceInvoice.getInvoiceById(id).subscribe({
        next: (r: InvoiceModel) => {
          this.formDebitMemo.patchValue({
            dateTime: r.dateTime,
            type: this.mapInvoiceType(r.type),
            customerAddress: r.customerAddress,
            customerCuit: r.customerCuit,
            customerName: r.customerName,
            customerDni: '',
            invoiceNumber: r.invoiceNumber,
            observation: r.observation
          });
          this.customerId.set(r.customerId),
            this.userId.set(r.userId),
            // this.formDebitMemo.controls['type'].setValue(r.type),
            //   this.type1 = r.type,
            //   this.formDebitMemo.controls['invoiceNumber'].setValue(r.invoiceNumber),
            //   this.customerId = r.customerId
            // this.formDebitMemo.controls['address'].setValue(r.customerAddress),
            //   this.formDebitMemo.controls['customerCuit'].setValue(r.customerCuit),
            //   this.formDebitMemo.controls['customerName'].setValue(r.customerName),
            //   this.ivaTotal = r.ivaTotal,
            //   this.total = r.total,
            //   this.userId.set(r.userId),
            //   this.subTotal = r.total - r.ivaTotal;
            /**parse a Grilla */
            this.dataGrid.set(r.invoiceDetails.map((modelDetail: InvoiceDetails, index) => {
              return debitMemoGridFromInvoiceParser(modelDetail, index)
            }));

          /**Parseo al back */
          this.dataDetails.set(r.invoiceDetails.map((modelDetail: InvoiceDetails, index) => {
            return debitMemoDetailFromInvoiceParser(modelDetail, index)
          }));

          this.isLoading.set(false);
        },

        error: () => {
          this.isLoading.set(false);
          this.dataDetails.set([]);
          this.dataGrid.set([]);
        }
      });
    }
  }

  save(): void {
    if (this.isValidForm(this.formDebitMemo)) {
      if (this.selectedDni && this.formDebitMemo.controls['customerDni'].value.length < 8) {
        return this.showMessageError('DNI Invalido');
      };
      if (this.dataDetails().length == 0) {
        this.showMessageError('No hay Productos Seleccionados');

      } else {
        this.dni = this.formDebitMemo.controls['customerDni'].value
        const model: DebitMemoModel = {
          id: 0,
          customerId: this.customerId(),
          userId: this.userId(),
          invoiceId: this.id() ?? 0,
          invoiceNumber: this.formDebitMemo.controls['invoiceNumber'].value,
          customerName: this.formDebitMemo.controls['customerName'].value,
          customerCuit: this.selectedDni ? this.dni.toString() : this.formDebitMemo.controls['customerCuit'].value,
          customerAddress: this.formDebitMemo.controls['customerAddress'].value,
          observation: this.formDebitMemo.controls['observation'].value,
          dateTime: this.formDebitMemo.controls['dateTime'].value,
          type: this.formDebitMemo.controls['type'].value,
          total: this.total(),
          debitMemoNumber: 0,
          ivaTotal: this.ivaTotal(),
          caeExpirationTime: null,
          cae: null,
          version: InvoiceVersion.Arca,
          integrationSuccess: false,
          debitMemoDetails: this.dataDetails(),
        };
        this.isSaving.set(true);
        this.service.saveDebitMemo(model)
          .subscribe({
            next: (r) => {
              this.showNotificationSuccess(
                'Guardado correcto',
                `Nota de Débito creada correctamente`

              );
              this.isSaving.set(false);
              this.router.navigate(['/home/debits']);
            },
            error: (r) => {
              this.isSaving.set(false);
              this.showMessageError(r.error.descripcion)
            }
          });
      }
    }
  }

  ivaCalculate(data: number, iva: number): number {
    let newIva = 1 + (iva / 100);
    return (data / newIva);
  }

  currencyFormat(data: any): string {
    return formatCurrency(data, this.locale, '$', 'ARS', '1.1-2')
  }

  typeSelectedChange(id: any): void {
    this.typeSelectedId = id;
    if (id == 1) {
      this.invoiceA = true;
    } else {
      this.invoiceA = false;

    }
  }

  startEdit(id: number): void {
    this.editId.set(id);
  }

  startEditIva(id: number): void {
    this.editIdIva.set(id);
  }

  startEditProductPrice(id: number): void {
    this.editIdProductPrice.set(id);
  }

  stopEdit(): void {
    this.editId.set(null);
  };
  stopEditIva(): void {
    this.editIdIva.set(null);
  }

  stopEditProductPrice(): void {
    this.editIdProductPrice.set(null);
  }

  changeIvaValue(iva: number, id: number): void {
    let newIva = Number(iva);
    try {
      this.dataGrid.update(list =>
        list.map(detail =>
          detail.ownCode === id
            ? { ...detail, iva: newIva }
            : detail
        )
      );

      this.dataDetails.update(list =>
        list.map(detail =>
          detail.productId === id
            ? { ...detail, iva: newIva }
            : detail
        )
      );

      this.stopEditIva();

    } catch (error) {
      console.error(error);
    }
  }

  invoiceType(id: any): string {
    return eInvoiceType[id]
  }

  openComponentCustomer(): void {
    const drawerRefCustomer = this.drawerService.create<InvoiceCustomerSearchComponent, {}, CustomerModel>({
      nzTitle: 'Cliente',
      nzContent: InvoiceCustomerSearchComponent,
      nzSize: 'large',
      nzWidth: '90%',
      nzClosable: false
    });
    drawerRefCustomer.afterClose.subscribe({
      next: (data: CustomerModel | undefined) => {
        if (data != undefined) {
          this.customerId.set(data.id);
          this.formDebitMemo.patchValue({
            customerAddress: data.address,
            customerCuit: !isNil(data.cuit) ? data.cuit.replace(/[^a-zA-Z0-9 ]/g, '') : null,
            customerName: data.name,
            customerDni: data.dni
          });
        }
      },
      error: () => { }

    })
  }

  openComponentProduct(): void {
    if (this.isValidForm(this.formDebitMemo)) {
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
    } else { return; }
  }

  searchProduct(): void {
    if (!this.isValidForm(this.formDebitMemo)) { return; }

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
      },
      error: () => {
        this.isLoading.set(false);
        this.formProductSearch.controls['productSearchFilter'].setValue('');
      }
    })

  }

  searchCustomer(): void {
    this.cuit =
      this.formDebitMemo.controls['customerCuit'].value;
    if (this.cuit == '00') {
      this.formDebitMemo.controls['customerAddress'].setValue('S/D');
      this.formDebitMemo.controls['customerCuit'].setValue('99999999995');
      this.formDebitMemo.controls['customerName'].setValue('-');
      this.customerId.set(this.serviceUser.currentUser()?.id ?? 0);
      return;
    } else {
      if (this.cuit.length >= 6) {
        this.serviceEntity.getByCuit(this.cuit).subscribe({
          next: (data) => {
            this.formDebitMemo.controls['customerAddress'].setValue(data.address);
            this.formDebitMemo.controls['customerCuit'].setValue(data.cuit);
            this.formDebitMemo.controls['customerName'].setValue(data.name);
          },
          error: () => { this.showMessageError('No se encontro Cliente'); }
        });
      }
    }
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

      if (this.isValidForm(this.formDebitMemo) && this.isValidForm(this.formCustomerSearch) &&
        this.isValidForm(this.formProductSearch)) {
        this.popComponent.showConfirmation();
      }
    } catch (error) { console.log(error); }
  }

  select() {
    this.selectedDni = !this.selectedDni;
    if (this.selectedDni) {
      this.dni = this.formDebitMemo.controls['customerDni'].value;
    } else {
      this.dni = null;
    }
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
    const model: DebitMemoGrid = debitMemoGridParser(newEditProduct, this.iva);
    this.dataGrid.update(item => [...item, model]);

    /* Parseo dato a Dto Factura Detalle */
    const modelDetail: DebitMemoDetails = debitMemoDetailParser(newEditProduct, this.iva);
    this.dataDetails.update(item => [...item, modelDetail]);

    this.isLoading.set(false);
    this.formProductSearch.controls['productSearchFilter'].setValue('');
  }

  stopEditProductName(): void {
    this.editIdProductName.set(null);
  }

  startEditProductName(id: number): void {
    this.editIdProductName.set(id);
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
      const gridDetail: DebitMemoGrid = debitMemoGridParser(product, this.iva);
      const detail: DebitMemoDetails = debitMemoDetailParser(product, this.iva);

      this.dataGrid.update(list => [...list, gridDetail]);
      this.dataDetails.update(list => [...list, detail]);
    }
  }

  toggleEdit() {
    this.isEditMode.set(!this.isEditMode());
    if (this.isEditMode()) {
      this.formDebitMemo.enable();
      this.formProductSearch.enable();
      this.isInvoiceEditable.set(true);
    } else {
      this.formDebitMemo.disable();
      this.formProductSearch.disable();
      this.isInvoiceEditable.set(false);
    }
  }

  getDebitMemo(id: number): void {
    if (id != 0) {
      this.isLoading.set(true);
      this.service.getDebitMemoById(id).subscribe({
        next: (r) => {
          this.formDebitMemo.patchValue({
            dateTime: r.dateTime,
            type: this.mapInvoiceType(r.type),
            customerAddress: r.customerAddress,
            customerCuit: r.customerCuit,
            customerName: r.customerName,
            customerDni: '',
            invoiceNumber: r.invoiceNumber,
            observation: r.observation
          });
          this.dataGrid.set(r.debitMemoDetails.map((modelDetail: InvoiceDetails, index: number) => {
            return debitMemoGridFromInvoiceParser(modelDetail, index)
          }));
          this.userId.set(r.userId),
            // this.type = r.type
            // this.invoiceNumber = r.invoiceNumber
            //   this.debitMemoNumber = r.debitMemoNumber,
            //   this.userId = r.userId,
            //   this.dateTime = r.dateTime,
            //   this.debitMemoDetail = r.debitMemoDetails
            // this.isLoading = false;
            // this.version = r.version;
            // this.integrationSuccess = r.integrationSuccess;
            // this.cae = r.cae;
            // this.getTipo(r.type);
            this.cae.set(r.cae);
          this.integrationSuccess.set(r.integrationSuccess);
          this.version.set(r.version)
          this.isLoading.set(false);
        },
        error: () => {
          this.isLoading.set(false);
        },
      });
      this.getIntegrationLog(id);
    }
  }


  mapInvoiceType(type: eInvoiceType): number {
    switch (type) {
      case eInvoiceType.A:
      case eInvoiceType.RespMonotributo:
        return 1;

      case eInvoiceType.B:
      case eInvoiceType.C:
      case eInvoiceType.EXENTO:
        return 2;

      default:
        return 0;
    }
  }

  getIntegrationLog(id: number): Array<InvoiceLog> | any {
    if (id > 0) {
      this.logLoading.set(true);
      this.service.getIntegrationDebitLogById(id).subscribe({
        next: (r: Array<InvoiceLog>) => {
          this.invoiceLog.set(r);
          this.logLoading.set(false);
        },
        error: (e) => {
          this.logLoading.set(false);
        }
      });
    }
  }

  parserXML(data: string) {
    const xmlParser = new XMLParser();
    if (data == null) { return ''; }
    let parsed = xmlParser.parse(data);
    return JSON.stringify(parsed, null, 2)
  }
}