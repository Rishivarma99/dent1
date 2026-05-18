import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { environment } from '../../../../../environment/environment';
import { ApiResponse } from '../../../../shared/types/api/api-response';
import { unwrapApiResponse } from '../../../../shared/utils/api/unwrap-api-response';

export interface Patient {
  id: string;
  name: string;
  phone: string;
  createdAt: string;
}

@Injectable({ providedIn: 'root' })
export class PatientsApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/api/patients`;

  getAll(): Observable<Patient[]> {
    return this.http
      .get<ApiResponse<Patient[]>>(this.baseUrl)
      .pipe(map(unwrapApiResponse));
  }

  create(data: { name: string; phone: string }): Observable<unknown> {
    return this.http
      .post<ApiResponse<unknown>>(this.baseUrl, data)
      .pipe(map(unwrapApiResponse));
  }

  searchByPhone(phone: string): Observable<Patient[]> {
    return this.http
      .get<ApiResponse<Patient[]>>(`${this.baseUrl}/search`, { params: { phone } })
      .pipe(map(unwrapApiResponse));
  }
}
