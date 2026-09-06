import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { es_ES, provideNzI18n } from 'ng-zorro-antd/i18n';
import { registerLocaleData } from '@angular/common';
import es from '@angular/common/locales/es';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { authInterceptor } from './common/auth/interceptors/auth.interceptors';

registerLocaleData(es);

export const appConfig: ApplicationConfig = {
  providers: [provideRouter(routes), provideNzI18n(es_ES),
    provideHttpClient(
      withInterceptors([authInterceptor]))]
};
