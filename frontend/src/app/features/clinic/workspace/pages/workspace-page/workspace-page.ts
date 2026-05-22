import { ChangeDetectionStrategy, Component, computed, inject } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { Router } from '@angular/router';
import { ClinicPermissionsService } from '../../../../../core/services/clinic-permissions.service';
import { WorkspaceApiService } from '../../api/workspace-api.service';
import { PendingFollowUpsComponent } from '../../components/pending-follow-ups/pending-follow-ups';
import { TodayQueueComponent } from '../../components/today-queue/today-queue';
import { WorkspaceStatsComponent } from '../../components/workspace-stats/workspace-stats';

@Component({
  selector: 'app-workspace-page',
  imports: [WorkspaceStatsComponent, TodayQueueComponent, PendingFollowUpsComponent],
  templateUrl: './workspace-page.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class WorkspacePage {
  private readonly router = inject(Router);
  private readonly permissionsService = inject(ClinicPermissionsService);
  private readonly workspaceApi = inject(WorkspaceApiService);

  private readonly workspaceData = toSignal(this.workspaceApi.getTodayWorkspace(), {
    initialValue: {
      queue: [],
      followUps: [],
      stats: { cards: [] }
    }
  });

  protected readonly queueItems = computed(() => this.workspaceData().queue);
  protected readonly followUps = computed(() => this.workspaceData().followUps);
  protected readonly stats = computed(() => this.workspaceData().stats);

  protected readonly permissions = this.permissionsService.capabilities;

  protected onStartVisit(_appointmentId: string): void {
    void this.router.navigate(['/patients']);
  }

  protected onScheduleFollowUps(): void {
    void this.router.navigate(['/appointments']);
  }
}
