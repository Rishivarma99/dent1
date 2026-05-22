export type WorkspaceQueueStatus = 'completed' | 'in-progress' | 'arrived' | 'scheduled';
export type WorkspaceFollowUpType = 'Suggested' | 'Pending';

export interface WorkspaceQueueItem {
  readonly id: number;
  readonly appointmentId: string;
  readonly name: string;
  readonly time: string;
  readonly status: WorkspaceQueueStatus;
  readonly service: string;
}

export interface WorkspaceFollowUpItem {
  readonly id: number;
  readonly name: string;
  readonly type: WorkspaceFollowUpType;
  readonly days: string;
  readonly reason: string;
}

export interface WorkspaceStatCard {
  readonly label: string;
  readonly value: string;
  readonly hint: string;
  readonly icon: string;
  readonly tone: 'primary' | 'info' | 'warning' | 'danger';
}

export interface WorkspaceStats {
  readonly cards: readonly WorkspaceStatCard[];
}
