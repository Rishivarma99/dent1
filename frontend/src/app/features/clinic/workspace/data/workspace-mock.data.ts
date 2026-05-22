import {
  WorkspaceFollowUpItem,
  WorkspaceQueueItem,
  WorkspaceStats
} from '../models/workspace.models';

/** Mock data scoped to the logged-in clinician until workspace API exists. */
export const WORKSPACE_MOCK_QUEUE: readonly WorkspaceQueueItem[] = [
  {
    id: 1,
    appointmentId: 'A001',
    name: 'Michael Thompson',
    time: '09:00 AM',
    status: 'completed',
    service: 'Root Canal'
  },
  {
    id: 2,
    appointmentId: 'A002',
    name: 'Sarah Martinez',
    time: '09:30 AM',
    status: 'in-progress',
    service: 'Cleaning'
  },
  {
    id: 3,
    appointmentId: 'A003',
    name: 'James Wilson',
    time: '10:00 AM',
    status: 'arrived',
    service: 'Consultation'
  },
  {
    id: 4,
    appointmentId: 'A004',
    name: 'Emily Davis',
    time: '10:30 AM',
    status: 'scheduled',
    service: 'Filling'
  },
  {
    id: 5,
    appointmentId: 'A005',
    name: 'Robert Brown',
    time: '11:00 AM',
    status: 'scheduled',
    service: 'Extraction'
  }
];

export const WORKSPACE_MOCK_FOLLOW_UPS: readonly WorkspaceFollowUpItem[] = [
  {
    id: 1,
    name: 'Patricia Miller',
    type: 'Suggested',
    days: '2 days ago',
    reason: 'Root Canal Follow-up'
  },
  {
    id: 2,
    name: 'Mark Anderson',
    type: 'Pending',
    days: '5 days ago',
    reason: 'Extraction Check'
  },
  {
    id: 3,
    name: 'Lisa Garcia',
    type: 'Suggested',
    days: '1 week ago',
    reason: 'Crown Adjustment'
  }
];

export function buildWorkspaceStatsFromQueue(
  queue: readonly WorkspaceQueueItem[],
  followUpCount: number
): WorkspaceStats {
  const completed = queue.filter((item) => item.status === 'completed').length;
  const inQueue = queue.filter(
    (item) => item.status !== 'completed'
  ).length;
  const inProgress = queue.filter((item) => item.status === 'in-progress').length;

  return {
    cards: [
      {
        label: "Today's Appointments",
        value: String(queue.length),
        hint: `${completed} completed`,
        icon: 'pi-calendar',
        tone: 'primary'
      },
      {
        label: 'In Queue',
        value: String(inQueue),
        hint: `${inProgress} in progress`,
        icon: 'pi-clock',
        tone: 'warning'
      },
      {
        label: 'Completed Today',
        value: String(completed),
        hint: 'Your visits',
        icon: 'pi-check-circle',
        tone: 'info'
      },
      {
        label: 'Pending Follow-ups',
        value: String(followUpCount),
        hint: 'Needs attention',
        icon: 'pi-exclamation-circle',
        tone: 'danger'
      }
    ]
  };
}
