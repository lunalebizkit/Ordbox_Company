import { ReactiveFormsModule } from '@angular/forms';
import { FormsModule } from '@angular/forms';

import { NzPageHeaderModule } from 'ng-zorro-antd/page-header';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzModalModule } from 'ng-zorro-antd/modal';
import { NzSpinModule } from 'ng-zorro-antd/spin';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzTagModule } from 'ng-zorro-antd/tag';
import { NzTypographyModule } from 'ng-zorro-antd/typography';
import { NzImageModule } from 'ng-zorro-antd/image';

import { PopupConfirmationComponent } from './components/popup-confirmation/popup-confirmation.component';
import { HeaderOperationsButtonsComponent } from './components/headers/buttons.oparations.header.component';
import { BaseComponent } from './components/base/base.component';
import { AuthModule } from './auth/auth.module';
import { PermissionDirective } from './directives/permission.directive';
import { AuthModalComponent } from './auth/auth-modal/auth-modal.component';
import { CuitPipe } from './pipes/cuit.pipe';
import { NoCommaPipe } from './pipes/no-comma.pipe';
import { ButtonOperationFooter } from './components/footers/button.operation.footer.component';
import { InvoiceTypePipe } from './pipes/invoice-type.pipe';
import { SearchFilterComponent } from './components/search-filter/search.filter.component';
import { NzCollapseModule } from 'ng-zorro-antd/collapse';
import { IvaConditionTypePipe } from './pipes/ivacondition-type';
import { SearchCustomFilterComponent } from './components/search-custom-filter/search.custom.filter.component';
import { NzDatePickerModule } from 'ng-zorro-antd/date-picker';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

@NgModule({
  imports: [
    IvaConditionTypePipe,
    ReactiveFormsModule,
    FormsModule,
    CommonModule,
    NoCommaPipe,
    NzSelectModule,
    NzIconModule,
    NzFormModule,
    NzInputModule,
    NzPageHeaderModule,
    NzModalModule,
    NzSpinModule,
    NzButtonModule,
    NzTagModule,
    NzTypographyModule,
    NzImageModule,
    HeaderOperationsButtonsComponent,
    NzModalModule,
    BaseComponent
  ],
  exports: [
    IvaConditionTypePipe,
    ReactiveFormsModule,
    FormsModule,
    CommonModule,
    NoCommaPipe,
    NzSelectModule,
    NzIconModule,
    NzFormModule,
    NzInputModule,
    NzPageHeaderModule,
    NzModalModule,
    NzSpinModule,
    NzButtonModule,
    NzTagModule,
    NzTypographyModule,
    NzImageModule,
    AuthModule,    
    HeaderOperationsButtonsComponent,
    NzModalModule,
    BaseComponent
  ],
  declarations: [],
  providers: [],
})
export class AppCommonModule {}
