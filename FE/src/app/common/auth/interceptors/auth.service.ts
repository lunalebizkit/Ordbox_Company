import { Injectable, EventEmitter, Output } from '@angular/core';
import { BehaviorSubject, Observable, tap } from "rxjs";
import { AuthUserModel } from "../models/auth-user.model";
import { HttpClient } from '@angular/common/http';
import { ApiService } from '../../services/api.base.service';
import { SecurityAuthService } from '../../../pages/auth/security-auth.service';

@Injectable({
  providedIn: 'root'
})

export class AuthService {

  /* Modal para log in de usuario */
  // @Output() activateAuthModal = new EventEmitter();

  /** Usuario de la aplicacion*/
  public user: BehaviorSubject<AuthUserModel>;

  /**
   * Constructor
   */
  constructor(
    private http: HttpClient,
    private service: SecurityAuthService
  ) {
    this.user = new BehaviorSubject<AuthUserModel>(JSON.parse(localStorage.getItem('auth-user')!));

  }
  /* Medtodo para setear Token y Obtener */
  public set tokenLS(token: string) {
    localStorage.setItem('token', JSON.stringify(token));
  }
  public get tokenLS(): string {
    return JSON.parse(localStorage.getItem('token')!);
  }
  /* Medtodo para setear RefreshToken y Obtener */
  public set refreshTokenLS(refreshtoken: string) {
    localStorage.setItem('refreshtoken', JSON.stringify(refreshtoken));
  }

  public get refreshTokenLS(): string {
    return JSON.parse(localStorage.getItem('refreshtoken')!);
  }
  /* Metodo para setea usuario y obtener */
  public get currentUser(): AuthUserModel {
    return this.user.value;
  }
  public set currentUser(user: AuthUserModel) {
    localStorage.setItem('auth-user', JSON.stringify(user));
    this.user = new BehaviorSubject<AuthUserModel>(user);
  }

  private saveTokens(response: AuthUserModel): void {
    this.tokenLS = response.token;
    this.refreshTokenLS = response.refreshToken;
    this.currentUser = response;
  }

  /**
   * Metodo para desloguear el usuario
   */
  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('refreshtoken');
    localStorage.removeItem('auth-user');
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

}
