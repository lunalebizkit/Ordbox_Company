import { Component, OnInit, signal } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NzDrawerModule, NzDrawerRef } from 'ng-zorro-antd/drawer';
import { NzTableModule, NzTableQueryParams } from 'ng-zorro-antd/table';
import { EntityService } from '../../customers/customer.service';
import { CustomerModel } from '../../customers/model/customer.model';
import { NzLayoutModule } from 'ng-zorro-antd/layout';
import { NzCollapseModule } from 'ng-zorro-antd/collapse';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzSelectModule } from 'ng-zorro-antd/select';

@Component({
  selector: 'app-invoice-customer-search',
  templateUrl: './invoice-customer-search.component.html',
  styleUrls: ['./invoice-customer-search.component.css'],
imports:[NzLayoutModule, NzCollapseModule, NzFormModule, NzInputModule, FormsModule, NzSelectModule, NzTableModule, NzDrawerModule, NzButtonModule, ReactiveFormsModule]
})
export class InvoiceCustomerSearchComponent implements OnInit {

  timeout!: any;
  allCustomer = signal<CustomerModel[]>([]);
  customer= signal<CustomerModel | null>(null);
  
  formSearch!: FormGroup;
  queryParams = {
    filter: '',
    page: 0,
    pageSize: 20
  };
  totalItems = signal<number>(0);
  loading = signal<boolean>(false);
  customerId!: number;
  
  constructor(
    private drawerRef: NzDrawerRef<string>,
    private serviceEntity: EntityService,
    private fb: FormBuilder) {
      this.formSearch = this.fb.group({          
                    
      })
    } 

  ngOnInit(): void {
   }

  close(): void {
    this.drawerRef.close(this.customer());  
  }
  /*
** Evento de busqueda datos en el server
*/
  onSearch(): void {
    clearTimeout(this.timeout);
    this.timeout = setTimeout(() => {

      if (this.queryParams.filter.length > 2) {
        this.getAllCustomer();
      }      
      
    }, 1000);
  }
 /*
  ** Evento que se ejecuta ante algun cambio en la grillas (sorting,paging or filtering)
  */
  onQueryParamsChange(params: NzTableQueryParams): void {
    this.queryParams.page = params.pageIndex -1;
    this.queryParams.pageSize = params.pageSize;
    this.getAllCustomer();
  }

  getAllCustomer(): void {
    this.loading.set(true);
    this.serviceEntity.getCustomers(this.queryParams).subscribe({
      next: (r) => {
        this.allCustomer.set(r.data);
        this.totalItems.set(r.totalCount);
        this.loading.set(false);
      },
      error: () => {
        this.allCustomer.set([]);
       this.loading.set(false);
      }
    })
  }


 selecccion(dato: any){  
 this.customerId= dato.composedPath()[1].id;
  this.customer.set(this.allCustomer().filter(id => id.id == this.customerId)[0]);
  this.close();
 }
}
