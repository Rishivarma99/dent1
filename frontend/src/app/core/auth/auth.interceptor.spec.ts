import { HttpClient } from '@angular/common/http';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideZonelessChangeDetection } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { environment } from '../../../environment/environment';
import { authInterceptor } from './auth.interceptor';
import { skipAuth } from './auth-context';
import { TokenStorage } from './token-storage';

describe('authInterceptor', () => {
  let http: HttpClient;
  let httpMock: HttpTestingController;
  let storage: TokenStorage;

  beforeEach(() => {
    sessionStorage.clear();

    TestBed.configureTestingModule({
      providers: [
        provideZonelessChangeDetection(),
        provideHttpClient(withInterceptors([authInterceptor])),
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

  it('attaches the bearer token to our API requests', () => {
    storage.setTokens('access-token', 'refresh-token');

    http.get(`${environment.apiUrl}/api/patients`).subscribe();

    const req = httpMock.expectOne(`${environment.apiUrl}/api/patients`);
    expect(req.request.headers.get('Authorization')).toBe('Bearer access-token');
    req.flush({ success: true, data: [], error: null });
  });

  it('skips auth when the request explicitly opts out', () => {
    storage.setTokens('access-token', 'refresh-token');

    http.post(`${environment.apiUrl}/api/auth/login`, { usernameOrPhone: 'alice', password: 'password' }, skipAuth()).subscribe();

    const req = httpMock.expectOne(`${environment.apiUrl}/api/auth/login`);
    expect(req.request.headers.has('Authorization')).toBeFalse();
    req.flush({ success: true, data: null, error: null });
  });

  it('does not attach tokens to third-party hosts', () => {
    storage.setTokens('access-token', 'refresh-token');

    http.get('https://maps.googleapis.com/maps/api/geocode/json').subscribe();

    const req = httpMock.expectOne('https://maps.googleapis.com/maps/api/geocode/json');
    expect(req.request.headers.has('Authorization')).toBeFalse();
    req.flush({});
  });
});
