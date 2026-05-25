import { Component } from '@angular/core';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideZonelessChangeDetection } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { provideRouter, Router } from '@angular/router';
import { environment } from '../../../environment/environment';
import { authInterceptor } from './auth.interceptor';
import { SKIP_AUTH } from './auth-context';
import { AuthService } from './auth.service';
import { AuthSessionResponse, AuthUser } from './auth.types';
import { TokenStorage } from './token-storage';

@Component({
  standalone: true,
  template: ''
})
class DummyComponent {}

describe('AuthService', () => {
  let service: AuthService;
  let httpMock: HttpTestingController;
  let router: Router;
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

    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);
    router = TestBed.inject(Router);
    storage = TestBed.inject(TokenStorage);

    spyOn(router, 'navigate').and.resolveTo(true);
  });

  afterEach(() => {
    httpMock.verify();
    sessionStorage.clear();
  });

  it('login stores tokens and updates the current user signal', () => {
    let receivedUser: AuthUser | undefined;

    service.login({ usernameOrPhone: 'alice', password: 'password' }).subscribe((response: AuthSessionResponse) => {
      receivedUser = response.user;
    });

    const req = httpMock.expectOne(`${environment.apiUrl}/api/auth/login`);
    expect(req.request.context.get(SKIP_AUTH)).toBeTrue();
    expect(req.request.headers.has('Authorization')).toBeFalse();

    req.flush({
      success: true,
      data: {
        accessToken: 'access-token',
        refreshToken: 'refresh-token',
        user
      },
      error: null
    });

    expect(receivedUser).toEqual(user);
    expect(storage.getAccess()).toBe('access-token');
    expect(storage.getRefresh()).toBe('refresh-token');
    expect(service.currentUser()).toEqual(user);
    expect(service.isAuthenticated()).toBeTrue();
    expect(service.hasRole('Admin')()).toBeTrue();
    expect(service.hasRole('Doctor')()).toBeFalse();
  });

  it('hardLogout clears auth state and redirects to the login page', () => {
    storage.setTokens('access-token', 'refresh-token');
    service.currentUser.set(user);

    service.hardLogout();

    expect(storage.getAccess()).toBeNull();
    expect(storage.getRefresh()).toBeNull();
    expect(service.currentUser()).toBeNull();
    expect(service.isAuthenticated()).toBeFalse();
    expect(router.navigate).toHaveBeenCalledWith(['/auth/login']);
  });
});
