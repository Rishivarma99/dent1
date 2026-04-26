import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs';
import { environment } from '../../../../environment/environment';

export interface Doctor {
  id: string;
  name: string;
  specialty: string;
}

interface ApiResponse<T> {
  success: boolean;
  data: T;
  error: unknown;
}

@Injectable({ providedIn: 'root' })
export class DoctorsService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/api/doctors`;

  private isApiResponse<T>(value: unknown): value is ApiResponse<T> {
    return typeof value === 'object' && value !== null && 'data' in value && 'success' in value;
  }

  private toArray<T>(value: unknown): T[] {
    const payload = this.isApiResponse<T[]>(value) ? value.data : value;
    return Array.isArray(payload) ? payload : [];
  }

  getAll() {
    return this.http.get<unknown>(this.baseUrl).pipe(map((response) => this.toArray<Doctor>(response)));
  }

  create(data: { name: string; specialty: string }) {
    return this.http.post<string>(this.baseUrl, data);
  }

  update(id: string, data: { name: string; specialty: string }) {
    return this.http.put(`${this.baseUrl}/${id}`, data);
  }

  delete(id: string) {
    return this.http.delete(`${this.baseUrl}/${id}`);
  }
}
