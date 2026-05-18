import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { map, Observable } from 'rxjs';
import { environment } from '../../../../environment/environment';
import { ApiResponse } from '../../../shared/types/api/api-response';
import { unwrapApiResponse } from '../../../shared/utils/api/unwrap-api-response';
import { AuthResponseDto } from './dtos/auth-response.dto';
import { LoginRequest } from './requests/login.request';

@Injectable({ providedIn: 'root' })
export class AuthApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/api/auth`;

  login(request: LoginRequest): Observable<AuthResponseDto> {
    return this.http
      .post<ApiResponse<AuthResponseDto>>(`${this.baseUrl}/login`, request)
      .pipe(map(unwrapApiResponse));
  }

  refresh(refreshToken: string): Observable<AuthResponseDto> {
    return this.http
      .post<ApiResponse<AuthResponseDto>>(`${this.baseUrl}/refresh`, { refreshToken })
      .pipe(map(unwrapApiResponse));
  }
}
