import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { WorkspaceQueueItem } from '../../models/workspace.models';
import {
  queueStatusBadgeClass,
  queueStatusIcon,
  queueStatusIconClass,
  queueStatusLabel
} from '../../utils/queue-status.util';

@Component({
  selector: 'app-today-queue',
  imports: [ButtonModule],
  templateUrl: './today-queue.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class TodayQueueComponent {
  readonly items = input.required<readonly WorkspaceQueueItem[]>();
  readonly canStartVisit = input(false);

  readonly startVisit = output<string>();

  protected readonly queueStatusIconClass = queueStatusIconClass;
  protected readonly queueStatusIcon = queueStatusIcon;
  protected readonly queueStatusBadgeClass = queueStatusBadgeClass;
  protected readonly queueStatusLabel = queueStatusLabel;

  protected onStartVisit(appointmentId: string): void {
    this.startVisit.emit(appointmentId);
  }
}
