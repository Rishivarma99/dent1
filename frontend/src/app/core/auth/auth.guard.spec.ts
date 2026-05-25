import { Component, computed, signal } from '@angular/core';
import { provideZonelessChangeDetection } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { provideRouter, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { AuthService } from './auth.service';
import { authGuard } from './auth.guard';
import { roleGuard } from './role.guard';
import { AuthUser } from './auth.types';

@Component({
  standalone: true,
  template: ''
})
class DummyComponent {}

describe('auth guards', () => {
  let router: Router;

  const currentUser = signal<AuthUser | null>(null);
  const authServiceStub = {
    currentUser,
    isAuthenticated: computed(() => currentUser() !== null),
    hasRole: (role: string) => computed(() => currentUser()?.roles.includes(role) ?? false),
    logout: jasmine.createSpy('logout')
  };

  beforeEach(() => {
    currentUser.set(null);

    TestBed.configureTestingModule({
      providers: [
        provideZonelessChangeDetection(),
        provideRouter([
          { path: 'auth/login', component: DummyComponent },
          { path: 'forbidden', component: DummyComponent }
        ]),
        { provide: AuthService, useValue: authServiceStub }
      ]
    });

    router = TestBed.inject(Router);
  });

  it('authGuard allows authenticated users', () => {
    currentUser.set({
      id: '8f7457f1-8b47-49e9-98b9-5442f9eb6977',
      name: 'Alice Johnson',
      email: 'alice@dentova.com',
      roles: ['Admin']
    });

    const result = TestBed.runInInjectionContext(() =>
      authGuard({} as never, { url: '/patients' } as RouterStateSnapshot)
    );

    expect(result).toBeTrue();
  });

  it('authGuard redirects anonymous users to the login page with a returnUrl', () => {
    const result = TestBed.runInInjectionContext(() =>
      authGuard({} as never, { url: '/patients' } as RouterStateSnapshot)
    );

    expect(result instanceof UrlTree).toBeTrue();
    expect(router.serializeUrl(result as UrlTree)).toBe('/auth/login?returnUrl=%2Fpatients');
  });

  it('roleGuard allows users with at least one required role', () => {
    currentUser.set({
      id: '8f7457f1-8b47-49e9-98b9-5442f9eb6977',
      name: 'Alice Johnson',
      email: 'alice@dentova.com',
      roles: ['Admin']
    });

    const result = TestBed.runInInjectionContext(() => roleGuard(['Admin', 'Doctor'])({} as never, {} as never));

    expect(result).toBeTrue();
  });

  it('roleGuard redirects users without the required role', () => {
    currentUser.set({
      id: '8f7457f1-8b47-49e9-98b9-5442f9eb6977',
      name: 'Alice Johnson',
      email: 'alice@dentova.com',
      roles: ['Receptionist']
    });

    const result = TestBed.runInInjectionContext(() => roleGuard(['Admin'])({} as never, {} as never));

    expect(result instanceof UrlTree).toBeTrue();
    expect(router.serializeUrl(result as UrlTree)).toBe('/forbidden');
  });
});
