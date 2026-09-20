import { Inject, LOCALE_ID, ViewChild } from '@angular/core';
import { Component, ElementRef, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { formatCurrency, formatDate } from '@angular/common';
import { ProductService } from '../product.service'
import { ProductReport } from '../model/product.report.model';
import { BaseComponent } from '../../../../common/components/base/base.component';
import { HeaderOperationsButtonsComponent } from '../../../../common/components/headers/buttons.oparations.header.component';
import { PopupConfirmationComponent } from '../../../../common/components/popup-confirmation/popup-confirmation.component';
import { NzLayoutModule } from 'ng-zorro-antd/layout';
import { NzPageHeaderModule } from 'ng-zorro-antd/page-header';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzSpinModule } from 'ng-zorro-antd/spin';
import { NzCollapseModule } from 'ng-zorro-antd/collapse';
import { NzTableModule } from 'ng-zorro-antd/table';

@Component({
    selector: 'app-products-report',
    templateUrl: './products-report.component.html',
    styleUrls: ['./products-report.component.css'],
    imports: [NzLayoutModule, NzPageHeaderModule, NzButtonModule, NzSpinModule, NzCollapseModule, NzTableModule]
})

export class ProductsReportComponent extends BaseComponent implements OnInit {
    @ViewChild('popup') popupComponent!: PopupConfirmationComponent;
    @ViewChild('header') headerComponent!: HeaderOperationsButtonsComponent;
    @ViewChild('pop') popComponent!: PopupConfirmationComponent;
    

    loading!: boolean;
     finishPage : number = 0;
     actualPage: number = 0;
     selectedIndex!: number;
     selectedProduct: any;
     index!: number;

    productsReportList!: ProductReport;

    constructor(
        notificacionService: NzNotificationService,
        el: ElementRef,
        message: NzMessageService,
        private router: Router,
        private service: ProductService,
        @Inject(LOCALE_ID) public locale: string,
    ) {
        super(notificacionService, el, message);
    }
    ngOnInit(): void {
    }
    msjConfirmOk() {
        try {
          this.popComponent.showConfirmation()
        } catch (error) { }
      }
    
      productsReport() {
       this.loading = true;
        this.service.productsReport().subscribe({
          next: (r) => {  
           this.productsReportList = r;
            this.finishPage = this.productsReportList.products.length/100;
            this.actualPage= 0;
            this.loading = false;         
          },
          error: () => {
            this.loading = false;
          },
        });
      }
    
      formaterDate(date: string | number | Date): string {
        return formatDate(date, 'MM/dd/YYYY', this.locale);
      }
      onScroll() {
        if (this.actualPage < this.finishPage) {
          this.actualPage ++; 
        } else {
          console.log('No more lines. Finish page!');
        }
      }
       currencyFormat(data: any):string  {    
    return formatCurrency(data, this.locale, '$', 'ARS', '1.1-2')
  }
  
  onClick(datos: any, index: number): void {
    this.index = index;
    this.selectedIndex = index;
    this.selectedProduct = datos;
  }
  myNavegation(event: any) {
    switch (event.key) {
      case 'ArrowDown':
        let nextCell =
          this.productsReportList.products.length > this.selectedIndex
            ? ++this.selectedIndex
            : this.productsReportList.products.length;
        if (this.productsReportList.products[nextCell] !== undefined) {
          this.selectedProduct = this.productsReportList.products[nextCell];
          this.index = nextCell;
          document.getElementById(nextCell.toString())?.focus();
        }
        setTimeout(() => {
          this.onScroll;
        }, 9000)
        break;
      case 'ArrowUp':
        let previousCell = this.selectedIndex > 0 ? --this.selectedIndex : 0;
        if (this.productsReportList.products[previousCell] !== undefined) {
          this.selectedProduct = this.productsReportList.products[previousCell];
          this.index = previousCell;
          document.getElementById(previousCell.toString())?.focus();
        }
        break;
    }
  }
}