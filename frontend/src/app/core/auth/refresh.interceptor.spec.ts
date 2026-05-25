import { HttpClient } from '@angular/common/http';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideZonelessChangeDetection } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { of, Subject } from 'rxjs';
import { environment } from '../../../environment/environment';
import { AuthService } from './auth.service';
import { authInterceptor } from './auth.interceptor';
import { refreshInterceptor } from './refresh.interceptor';
import { skipAuth } from './auth-context';
import { TokenStorage } from './token-storage';

describe('refreshInterceptor', () => {
  let http: HttpClient;
  let httpMock: HttpTestingController;
  let storage: TokenStorage;
  let authService: jasmine.SpyObj<Pick<AuthService, 'refresh' | 'hardLogout'>>;

  beforeEach(() => {
    sessionStorage.clear();
    authService = jasmine.createSpyObj<Pick<AuthService, 'refresh' | 'hardLogout'>>('AuthService', [
      'refresh',
      'hardLogout'
    ]);
    authService.refresh.and.returnValue(
      of({
        accessToken: 'new-access-token',
        refreshToken: 'new-refresh-token'
      })
    );

    TestBed.configureTestingModule({
      providers: [
        provideZonelessChangeDetection(),
        { provide: AuthService, useValue: authService },
        provideHttpClient(withInterceptors([authInterceptor, refreshInterceptor])),
        provideHttpClientTesting()
      ]
    });

    http = TestBed.inject(HttpClient);
    httpMock = TestBed.inject(HttpTestingController);
    storage = TestBed.inject(TokenStorage);
  });

  afterEach(() => {
    httpMock.verify();
    sessionStorage.clear();
  });

  it('refreshes once and retries the failed protected request with the new token', () => {
    storage.setTokens('old-access-token', 'old-refresh-token');

    http.get(`${environment.apiUrl}/api/patients`).subscribe();

    const initialReq = httpMock.expectOne(`${environment.apiUrl}/api/patients`);
    expect(initialReq.request.headers.get('Authorization')).toBe('Bearer old-access-token');
    initialReq.flush('Unauthorized', { status: 401, statusText: 'Unauthorized' });

    expect(authService.refresh).toHaveBeenCalledTimes(1);

    const retriedReq = httpMock.expectOne(`${environment.apiUrl}/api/patients`);
    expect(retriedReq.request.headers.get('Authorization')).toBe('Bearer new-access-token');
    retriedReq.flush({ success: true, data: [], error: null });

    expect(storage.getAccess()).toBe('new-access-token');
    expect(storage.getRefresh()).toBe('new-refresh-token');
    expect(authService.hardLogout).not.toHaveBeenCalled();
  });

  it('does not try to refresh requests that opted out of auth', () => {
    const errorSpy = jasmine.createSpy('error');

    http
      .post(`${environment.apiUrl}/api/auth/login`, { usernameOrPhone: 'alice', password: 'password' }, skipAuth())
      .subscribe({ error: errorSpy });

    const req = httpMock.expectOne(`${environment.apiUrl}/api/auth/login`);
    req.flush('Unauthorized', { status: 401, statusText: 'Unauthorized' });

    expect(authService.refresh).not.toHaveBeenCalled();
    expect(errorSpy).toHaveBeenCalled();
  });

  it('releases queued requests when refresh fails', () => {
    const refreshSubject = new Subject<{ accessToken: string; refreshToken: string }>();
    authService.refresh.and.returnValue(refreshSubject);

    const firstErrorSpy = jasmine.createSpy('firstError');
    const secondErrorSpy = jasmine.createSpy('secondError');

    storage.setTokens('old-access-token', 'old-refresh-token');

    http.get(`${environment.apiUrl}/api/patients`).subscribe({ error: firstErrorSpy });
    http.get(`${environment.apiUrl}/api/doctors`).subscribe({ error: secondErrorSpy });

    const firstReq = httpMock.expectOne(`${environment.apiUrl}/api/patients`);
    const secondReq = httpMock.expectOne(`${environment.apiUrl}/api/doctors`);

    firstReq.flush('Unauthorized', { status: 401, statusText: 'Unauthorized' });
    secondReq.flush('Unauthorized', { status: 401, statusText: 'Unauthorized' });

    expect(authService.refresh).toHaveBeenCalledTimes(1);

    refreshSubject.error(new Error('refresh failed'));

    expect(firstErrorSpy).toHaveBeenCalled();
    expect(secondErrorSpy).toHaveBeenCalled();
    expect(authService.hardLogout).toHaveBeenCalled();
  });
});
