import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { BehaviorSubject, catchError, filter, switchMap, take, throwError } from 'rxjs';
import { environment } from '../../../environment/environment';
import { AuthService } from './auth.service';
import { SKIP_AUTH } from './auth-context';
import { TokenStorage } from './token-storage';

type RefreshResult =
  | { status: 'success'; accessToken: string }
  | { status: 'failed'; error: unknown }
  | null;

let refreshing = false;
const refreshed$ = new BehaviorSubject<RefreshResult>(null);

export const refreshInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);
  const tokens = inject(TokenStorage);
  const isOurApi = req.url.startsWith(environment.apiUrl) || req.url.startsWith('/api/');

  return next(req).pipe(
    catchError((error) => {
      if (!isOurApi || error.status !== 401 || req.context.get(SKIP_AUTH)) {
        return throwError(() => error);
      }

      if (!refreshing) {
        refreshing = true;
        refreshed$.next(null);

        return auth.refresh().pipe(
          switchMap((tokenPair) => {
            tokens.setTokens(tokenPair.accessToken, tokenPair.refreshToken);
            refreshing = false;
            refreshed$.next({ status: 'success', accessToken: tokenPair.accessToken });

            return next(
              req.clone({
                setHeaders: {
                  Authorization: `Bearer ${tokenPair.accessToken}`
                }
              })
            );
          }),
          catchError((refreshError) => {
            refreshing = false;
            refreshed$.next({ status: 'failed', error: refreshError });
            auth.hardLogout();
            return throwError(() => refreshError);
          })
        );
      }

      return refreshed$.pipe(
        filter((result): result is Exclude<RefreshResult, null> => result !== null),
        take(1),
        switchMap((result) => {
          if (result.status === 'failed') {
            return throwError(() => result.error);
          }

          return next(
            req.clone({
              setHeaders: {
                Authorization: `Bearer ${result.accessToken}`
              }
            })
          );
        })
      );
    })
  );
};
