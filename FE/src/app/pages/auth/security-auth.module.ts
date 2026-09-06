import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NzGridModule } from 'ng-zorro-antd/grid';
import { LoginComponent } from './login/login.component';
import { SecurityAuthRoutingModule } from './security-auth.routing';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzDrawerModule } from 'ng-zorro-antd/drawer';
import { ReactiveFormsModule } from '@angular/forms';
import { SecurityAuthService } from './security-auth.service';
import {NzAvatarModule} from 'ng-zorro-antd/avatar';
import { NzLayoutModule } from 'ng-zorro-antd/layout';
import { NzMenuModule } from 'ng-zorro-antd/menu';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzDividerModule } from 'ng-zorro-antd/divider';
import { AppCommonModule } from '../../common/app.common.module';
import { NzTabsModule } from 'ng-zorro-antd/tabs';
import { NzTransferModule } from 'ng-zorro-antd/transfer';

@NgModule({
  imports: [],  
  exports: [],
  providers: [SecurityAuthService]
})
export class SecurityAuthModule { }
