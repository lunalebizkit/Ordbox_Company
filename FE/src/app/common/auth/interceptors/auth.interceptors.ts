import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from './auth.service';
import { catchError, throwError, switchMap } from 'rxjs';
import { NzNotificationService } from 'ng-zorro-antd/notification';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const notification = inject(NzNotificationService);
  const authService = inject(AuthService);
  const token = authService.tokenLS;

  if (req.url.includes('/Refresh')) {
    return next(req);
  }

  const authRequest = token
    ? req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    })
    : req;

  return next(authRequest).pipe(

    catchError((error: HttpErrorResponse) => {

      // Si JWT expiró
      if (error.status === 401) {
        notification.warning(
          `Sesión expirada`, '',
          { nzPlacement: 'bottomRight' }
        );

        return authService.refreshToken().pipe(

          switchMap(response => {
            notification.success(
              `Reconectando...`, '',
              { nzPlacement: 'bottomRight' }
            );
            const retryRequest = req.clone({
              setHeaders: {
                Authorization: `Bearer ${response.token}`
              }
            });

            return next(retryRequest);
          }),

          catchError(refreshError => {

            notification.error(
              `No se puedo reconectar`, 'Inicie sesión nuevamente',
              { nzPlacement: 'bottomRight' }
            );
            authService.logout();
            return throwError(() => refreshError);
          })
        );
      }

      return throwError(() => error);
    })
  );
};
