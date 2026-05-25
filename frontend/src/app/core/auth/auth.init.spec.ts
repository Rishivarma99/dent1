import { Component } from '@angular/core';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideZonelessChangeDetection } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { environment } from '../../../environment/environment';
import { authInit } from './auth.init';
import { authInterceptor } from './auth.interceptor';
import { AuthService } from './auth.service';
import { AuthUser } from './auth.types';
import { TokenStorage } from './token-storage';

@Component({
  standalone: true,
  template: ''
})
class DummyComponent {}

describe('authInit', () => {
  let httpMock: HttpTestingController;
  let authService: AuthService;
  let storage: TokenStorage;

  const user: AuthUser = {
    id: '8f7457f1-8b47-49e9-98b9-5442f9eb6977',
    name: 'Alice Johnson',
    email: 'alice@dentova.com',
    roles: ['Admin']
  };

  beforeEach(() => {
    sessionStorage.clear();

    TestBed.configureTestingModule({
      providers: [
        provideZonelessChangeDetection(),
        provideRouter([{ path: 'auth/login', component: DummyComponent }]),
        provideHttpClient(withInterceptors([authInterceptor])),
        provideHttpClientTesting()
      ]
    });

    httpMock = TestBed.inject(HttpTestingController);
    authService = TestBed.inject(AuthService);
    storage = TestBed.inject(TokenStorage);
  });

  afterEach(() => {
    httpMock.verify();
    sessionStorage.clear();
  });

  it('does nothing when there is no access token to rehydrate', async () => {
    await TestBed.runInInjectionContext(() => authInit());

    expect(authService.currentUser()).toBeNull();
  });

  it('rehydrates the current user from /auth/me when a token exists', async () => {
    storage.setTokens('access-token', 'refresh-token');

    const initPromise = TestBed.runInInjectionContext(() => authInit());

    const req = httpMock.expectOne(`${environment.apiUrl}/api/auth/me`);
    expect(req.request.headers.get('Authorization')).toBe('Bearer access-token');
    req.flush({ success: true, data: user, error: null });

    await initPromise;

    expect(authService.currentUser()).toEqual(user);
    expect(authService.isAuthenticated()).toBeTrue();
  });
});
