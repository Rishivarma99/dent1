import { WorkspaceQueueStatus } from '../models/workspace.models';

export function queueStatusIconClass(status: WorkspaceQueueStatus): string {
  const base = 'flex size-10 shrink-0 items-center justify-center rounded-full';
  switch (status) {
    case 'completed':
      return `${base} bg-success-soft text-success`;
    case 'in-progress':
      return `${base} bg-info-soft text-info`;
    case 'arrived':
      return `${base} bg-primary-soft text-primary`;
    default:
      return `${base} bg-surface-container text-text-placeholder`;
  }
}

export function queueStatusIcon(status: WorkspaceQueueStatus): string {
  switch (status) {
    case 'completed':
      return 'pi-check-circle';
    case 'in-progress':
      return 'pi-play-circle';
    default:
      return 'pi-clock';
  }
}

export function queueStatusBadgeClass(status: WorkspaceQueueStatus): string {
  const base = 'mt-1 inline-block rounded px-2 py-0.5 text-xs font-semibold';
  switch (status) {
    case 'completed':
      return `${base} bg-success-soft text-success`;
    case 'in-progress':
      return `${base} bg-info-soft text-info`;
    case 'arrived':
      return `${base} bg-primary-soft text-primary`;
    default:
      return `${base} bg-surface-container text-text-muted`;
  }
}

export function queueStatusLabel(status: WorkspaceQueueStatus): string {
  const labels: Record<WorkspaceQueueStatus, string> = {
    completed: 'Completed',
    'in-progress': 'In Progress',
    arrived: 'Arrived',
    scheduled: 'Scheduled'
  };
  return labels[status];
}
