import { NgModule } from '@angular/core';;
import { AuthGuard } from './permission/auth.guard';
import { PermissionService } from './permission/permission-manager.service';

@NgModule({
  declarations: [],
  imports: [],
  providers: [AuthGuard, PermissionService],
})
export class AuthModule {}