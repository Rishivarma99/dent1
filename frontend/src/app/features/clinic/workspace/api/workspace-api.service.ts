import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '../../../../../environment/environment';
import { ApiResponse } from '../../../../shared/types/api/api-response';
import { unwrapApiResponse } from '../../../../shared/utils/api/unwrap-api-response';
import {
  WorkspaceFollowUpItem,
  WorkspaceQueueItem,
  WorkspaceStats
} from '../models/workspace.models';

export interface WorkspaceTodayDto {
  readonly queue: readonly WorkspaceQueueItem[];
  readonly followUps: readonly WorkspaceFollowUpItem[];
  readonly stats: WorkspaceStats;
}

@Injectable({ providedIn: 'root' })
export class WorkspaceApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/api/workspace`;

  getTodayWorkspace(): Observable<WorkspaceTodayDto> {
    return this.http
      .get<ApiResponse<WorkspaceTodayDto>>(`${this.baseUrl}/today`)
      .pipe(map(unwrapApiResponse));
  }
}
