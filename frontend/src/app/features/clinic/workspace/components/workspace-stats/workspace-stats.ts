import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { WorkspaceStats } from '../../models/workspace.models';

@Component({
  selector: 'app-workspace-stats',
  templateUrl: './workspace-stats.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class WorkspaceStatsComponent {
  readonly stats = input.required<WorkspaceStats>();

  protected iconToneClass(tone: string): string {
    switch (tone) {
      case 'info':
        return 'bg-info-soft text-info';
      case 'warning':
        return 'bg-warning-soft text-warning';
      case 'danger':
        return 'bg-danger-soft text-danger';
      default:
        return 'bg-primary-soft text-primary';
    }
  }
}
