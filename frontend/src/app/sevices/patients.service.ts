import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs';
import { environment } from '../../environment/environment';

export interface Patient {
  id: string;
  name: string;
  phone: string;
  createdAt: string;
}

interface ApiResponse<T> {
  success: boolean;
  data: T;
  error: unknown;
}

@Injectable({ providedIn: 'root' })
export class PatientsService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/api/patients`;

  private isApiResponse<T>(value: unknown): value is ApiResponse<T> {
    return typeof value === 'object' && value !== null && 'data' in value && 'success' in value;
  }

  private toArray<T>(value: unknown): T[] {
    const payload = this.isApiResponse<T[]>(value) ? value.data : value;
    return Array.isArray(payload) ? payload : [];
  }

  getAll() {
    return this.http.get<unknown>(this.baseUrl).pipe(map((response) => this.toArray<Patient>(response)));
  }

  create(data: { name: string; phone: string }) {
    return this.http.post<string>(this.baseUrl, data);
  }

  searchByPhone(phone: string) {
    return this.http
      .get<unknown>(`${this.baseUrl}/search`, { params: { phone } })
      .pipe(map((response) => this.toArray<Patient>(response)));
  }
}
