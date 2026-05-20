import { ApplicationConfig, provideBrowserGlobalErrorListeners, provideZonelessChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { authInterceptor } from './core/interceptors/auth.interceptor';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { providePrimeNG } from 'primeng/config';
import { definePreset } from '@primeuix/themes';
import Aura from '@primeuix/themes/aura';

import { routes } from './app.routes';
import { environment } from '../environment/environment';
import { initializeApp, provideFirebaseApp } from '@angular/fire/app';
import { getAuth, provideAuth } from '@angular/fire/auth';

const DentovaPreset = definePreset(Aura, {
  semantic: {
    primary: {
      50: '#f0fdfb',
      100: '#ccfbef',
      200: '#99f6df',
      300: '#73fbbd',
      400: '#53dea3',
      500: '#1fb981',
      600: '#008f7a',
      700: '#006c49',
      800: '#005236',
      900: '#00422b',
      950: '#002113'
    },
    borderRadius: {
      none: '0',
      xs: '0.125rem',
      sm: '0.25rem',
      md: '0.5rem',
      lg: '0.75rem',
      xl: '1rem'
    },
    colorScheme: {
      light: {
        primary: {
          color: '{primary.700}',
          inverseColor: '#ffffff',
          hoverColor: '{primary.600}',
          activeColor: '{primary.800}'
        },
        highlight: {
          background: '#f0fdfb',
          focusBackground: '#ccfbef',
          color: '{primary.800}',
          focusColor: '{primary.900}'
        }
      },
      dark: {
        primary: {
          color: '{primary.400}',
          inverseColor: '{primary.950}',
          hoverColor: '{primary.300}',
          activeColor: '{primary.500}'
        },
        highlight: {
          background: 'rgba(83, 222, 163, 0.14)',
          focusBackground: 'rgba(83, 222, 163, 0.22)',
          color: '#e8f5f1',
          focusColor: '#f4fbf4'
        }
      }
    }
  }
});

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZonelessChangeDetection(),
    provideRouter(routes),
    provideHttpClient(withInterceptors([authInterceptor])),
    provideAnimationsAsync(),
    providePrimeNG({
      theme: {
        preset: DentovaPreset,
        options: {
          darkModeSelector: '[data-theme="dark"]'
        }
      }
    }),
    provideFirebaseApp(() => initializeApp(environment.firebase1)),
    provideAuth(() => getAuth())
  ]
};
