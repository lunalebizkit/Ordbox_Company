import {
  ActivatedRouteSnapshot,
  RouterStateSnapshot,
  Router,
  CanActivate,
} from '@angular/router';
import { Injectable } from '@angular/core';
import { AuthService } from '../interceptors/auth.service';
import { PermissionService } from './permission-manager.service';

@Injectable({
  providedIn: 'root'
})

export class AuthGuard implements CanActivate {
  constructor(
    private router: Router,
    private authService: AuthService,
    private permission: PermissionService
  ) {}

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    const user = this.authService.currentUser;

    if (!user) {
      this.router.navigate(['auth/login']);
      return false;
    } else {
      return this.permission.hasPermission(state.url);
    }
  }
}
