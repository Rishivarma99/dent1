import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { environment } from '../../../../../environment/environment';
import { ApiResponse } from '../../../../shared/types/api/api-response';
import { unwrapApiResponse } from '../../../../shared/utils/api/unwrap-api-response';

export interface Doctor {
  id: string;
  name: string;
  specialty: string;
}

@Injectable({ providedIn: 'root' })
export class DoctorsApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/api/doctors`;

  getAll(): Observable<Doctor[]> {
    return this.http
      .get<ApiResponse<Doctor[]>>(this.baseUrl)
      .pipe(map(unwrapApiResponse));
  }

  create(data: { name: string; specialty: string }): Observable<unknown> {
    return this.http
      .post<ApiResponse<unknown>>(this.baseUrl, data)
      .pipe(map(unwrapApiResponse));
  }

  update(id: string, data: { name: string; specialty: string }): Observable<Doctor> {
    return this.http
      .put<ApiResponse<Doctor>>(`${this.baseUrl}/${id}`, data)
      .pipe(map(unwrapApiResponse));
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
