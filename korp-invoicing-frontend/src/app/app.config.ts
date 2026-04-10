import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';

import { routes } from './app.routes';
import { provideAnimations } from '@angular/platform-browser/animations';
import { provideToastr } from 'ngx-toastr';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(), 
    provideRouter(routes), 
    provideHttpClient(), 
    provideAnimations(), 
    provideToastr({
      timeOut: 10000,
      closeButton: true,
      progressBar: true,
      preventDuplicates: true,
      newestOnTop: true,
      positionClass: 'toast-top-right'
    })
  ],
};
