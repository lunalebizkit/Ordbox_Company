import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from './auth.service';
import { catchError, throwError, switchMap } from 'rxjs';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
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

        return authService.refreshToken().pipe(

          switchMap(response => {

            const retryRequest = req.clone({
              setHeaders: {
                Authorization: `Bearer ${response.token}`
              }
            });

            return next(retryRequest);
          }),

          catchError(refreshError => {

            // authService.logout();

            return throwError(() => refreshError);
          })
        );
      }

      return throwError(() => error);
    })
  );
};
