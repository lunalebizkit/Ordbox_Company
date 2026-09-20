import { Injectable } from '@angular/core';
import { ApiService } from '../../common/services/api.base.service';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class SecurityAuthService {
  constructor(private api: ApiService) {}

  public login(user: any): Observable<any> {
    return this.api.post(`access/auth`, user, false);
  }
  public requestPasswordReset(passwordReset: any): Observable<any> {
    return this.api.post(`access/requestpasswordreset`, passwordReset, false);
  }

  public resetPassword(passwordReset: any): Observable<any> {
    return this.api.post(`access/resetpassword`, passwordReset, false);
  }

  public getUser(): Observable<any> {
    return this.api.get(`access/userAccount`);
  }
 
  public refreshToken(refreshToken: string | null): Observable<any> {
    return this.api.post(`access/Refresh`, {refreshToken}, false);
  }
}
