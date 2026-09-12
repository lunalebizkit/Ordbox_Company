import { Injectable, signal, computed } from '@angular/core';
import { Observable, tap } from "rxjs";
import { AuthUserModel } from "../models/auth-user.model";
import { SecurityAuthService } from '../../../pages/auth/security-auth.service';

@Injectable({
  providedIn: 'root'
})

export class AuthService {

  private userSignal = signal<AuthUserModel | null>(
    JSON.parse(localStorage.getItem('auth-user')!)
  );

  isLoggedIn = computed(() => !!this.userSignal());

  /**
   * Constructor
   */
  constructor(
    private service: SecurityAuthService,
  ) { }
  
  public get tokenLS(): string {
    return JSON.parse(localStorage.getItem('token')!);
  }  

  public get refreshTokenLS(): string {
    return JSON.parse(localStorage.getItem('refreshtoken')!);
  }
  
  private saveTokens(response: AuthUserModel): void {
    localStorage.setItem('token', JSON.stringify(response.token));
    localStorage.setItem('refreshtoken', JSON.stringify(response.refreshToken));
    localStorage.setItem('auth-user', JSON.stringify(response));
    this.userSignal.set(response);
  }

  /**
   * Metodo para desloguear el usuario
   */
  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('refreshtoken');
    localStorage.removeItem('auth-user');
    this.userSignal.set(null);
  }

  login(model: { userName: string; password: string }) {
    return this.service.login(model).pipe(
      tap((r: AuthUserModel) => {
        this.saveTokens(r);
      })
    );
  }

  refreshToken(): Observable<AuthUserModel> {
    const refreshToken = this.refreshTokenLS;

    if (!refreshToken) {
      throw new Error('No existe refresh token');
    }

    return this.service.refreshToken(refreshToken).pipe(
      tap((r: AuthUserModel) => {
        this.saveTokens(r);
      }
      ));
  }

  currentUser() {
    return this.userSignal();
  }
}