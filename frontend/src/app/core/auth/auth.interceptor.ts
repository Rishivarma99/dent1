import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { environment } from '../../../environment/environment';
import { SKIP_AUTH } from './auth-context';
import { TokenStorage } from './token-storage';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const isOurApi = req.url.startsWith(environment.apiUrl) || req.url.startsWith('/api/');
  if (!isOurApi || req.context.get(SKIP_AUTH)) {
    return next(req);
  }

  const accessToken = inject(TokenStorage).getAccess();
  if (!accessToken) {
    return next(req);
  }

  return next(
    req.clone({
      setHeaders: {
        Authorization: `Bearer ${accessToken}`
      }
    })
  );
};
