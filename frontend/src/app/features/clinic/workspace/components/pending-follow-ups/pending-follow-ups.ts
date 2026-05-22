import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { WorkspaceFollowUpItem, WorkspaceFollowUpType } from '../../models/workspace.models';

@Component({
  selector: 'app-pending-follow-ups',
  imports: [ButtonModule],
  templateUrl: './pending-follow-ups.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class PendingFollowUpsComponent {
  readonly items = input.required<readonly WorkspaceFollowUpItem[]>();
  readonly canSchedule = input(false);

  readonly scheduleFollowUps = output<void>();

  protected followUpIconClass(type: WorkspaceFollowUpType): string {
    const base = 'flex size-10 shrink-0 items-center justify-center rounded-full';
    return type === 'Suggested'
      ? `${base} bg-warning-soft text-warning`
      : `${base} bg-danger-soft text-danger`;
  }

  protected followUpIcon(type: WorkspaceFollowUpType): string {
    return type === 'Suggested' ? 'pi-user-plus' : 'pi-exclamation-circle';
  }

  protected followUpBadgeClass(type: WorkspaceFollowUpType): string {
    const base = 'rounded px-2 py-0.5 text-xs font-semibold';
    return type === 'Suggested'
      ? `${base} bg-warning-soft text-warning`
      : `${base} bg-danger-soft text-danger`;
  }

  protected onScheduleFollowUps(): void {
    this.scheduleFollowUps.emit();
  }
}
