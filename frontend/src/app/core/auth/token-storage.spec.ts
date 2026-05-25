import { provideZonelessChangeDetection } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { TokenStorage } from './token-storage';

describe('TokenStorage', () => {
  let storage: TokenStorage;

  beforeEach(() => {
    sessionStorage.clear();

    TestBed.configureTestingModule({
      providers: [provideZonelessChangeDetection()]
    });
    storage = TestBed.inject(TokenStorage);
  });

  afterEach(() => {
    sessionStorage.clear();
  });

  it('stores and reads both tokens from sessionStorage', () => {
    storage.setTokens('access-token', 'refresh-token');

    expect(storage.getAccess()).toBe('access-token');
    expect(storage.getRefresh()).toBe('refresh-token');
  });

  it('clears both tokens', () => {
    storage.setTokens('access-token', 'refresh-token');

    storage.clear();

    expect(storage.getAccess()).toBeNull();
    expect(storage.getRefresh()).toBeNull();
  });
});
