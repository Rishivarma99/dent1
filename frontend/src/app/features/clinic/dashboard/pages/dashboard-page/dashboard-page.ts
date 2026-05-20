import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { ButtonModule } from 'primeng/button';

type QueueStatus = 'completed' | 'in-progress' | 'arrived' | 'scheduled';
type FollowUpType = 'Suggested' | 'Pending';
type ActivityType = 'visit' | 'prescription' | 'appointment' | 'treatment';

interface QueueItem {
  id: number;
  appointmentId: string;
  name: string;
  time: string;
  status: QueueStatus;
  service: string;
}

interface UpcomingAppointment {
  id: number;
  name: string;
  time: string;
  service: string;
  doctor: string;
}

interface FollowUpItem {
  id: number;
  name: string;
  type: FollowUpType;
  days: string;
  reason: string;
}

interface ActivityItem {
  id: number;
  type: ActivityType;
  title: string;
  description: string;
  time: string;
  icon: string;
}

@Component({
  selector: 'app-dashboard-page',
  imports: [ButtonModule],
  templateUrl: './dashboard-page.html',
  styleUrl: './dashboard-page.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class DashboardPage {
  private readonly router = inject(Router);

  protected readonly queueItems: QueueItem[] = [
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

  protected readonly upcomingAppointments: UpcomingAppointment[] = [
    {
      id: 1,
      name: 'Jennifer Clark',
      time: '02:00 PM',
      service: 'Teeth Whitening',
      doctor: 'Dr. Sarah Johnson'
    },
    {
      id: 2,
      name: 'David Lee',
      time: '02:30 PM',
      service: 'Bridge Fitting',
      doctor: 'Dr. Sarah Johnson'
    },
    {
      id: 3,
      name: 'Amanda White',
      time: '03:00 PM',
      service: 'Crown Placement',
      doctor: 'Dr. Sarah Johnson'
    }
  ];

  protected readonly followUps: FollowUpItem[] = [
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
    },
    {
      id: 4,
      name: 'Tom Harris',
      type: 'Pending',
      days: '1 week ago',
      reason: 'Post-treatment'
    }
  ];

  protected readonly activities: ActivityItem[] = [
    {
      id: 1,
      type: 'visit',
      title: 'Visit Completed',
      description: 'Michael Thompson - Root Canal',
      time: '10 mins ago',
      icon: 'pi-check-circle'
    },
    {
      id: 2,
      type: 'prescription',
      title: 'Prescription Sent',
      description: 'Sarah Martinez - Pain Management',
      time: '25 mins ago',
      icon: 'pi-file'
    },
    {
      id: 3,
      type: 'appointment',
      title: 'Appointment Scheduled',
      description: 'New patient - Emma Wilson',
      time: '1 hour ago',
      icon: 'pi-calendar'
    },
    {
      id: 4,
      type: 'treatment',
      title: 'Treatment Started',
      description: 'James Wilson - Orthodontics',
      time: '2 hours ago',
      icon: 'pi-heart'
    }
  ];

  protected queueStatusIconClass(status: QueueStatus): string {
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

  protected queueStatusIcon(status: QueueStatus): string {
    switch (status) {
      case 'completed':
        return 'pi-check-circle';
      case 'in-progress':
        return 'pi-play-circle';
      default:
        return 'pi-clock';
    }
  }

  protected queueStatusBadgeClass(status: QueueStatus): string {
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

  protected queueStatusLabel(status: QueueStatus): string {
    const labels: Record<QueueStatus, string> = {
      completed: 'Completed',
      'in-progress': 'In Progress',
      arrived: 'Arrived',
      scheduled: 'Scheduled'
    };
    return labels[status];
  }

  protected followUpIconClass(type: FollowUpType): string {
    const base = 'flex size-10 shrink-0 items-center justify-center rounded-full';
    return type === 'Suggested'
      ? `${base} bg-warning-soft text-warning`
      : `${base} bg-danger-soft text-danger`;
  }

  protected followUpIcon(type: FollowUpType): string {
    return type === 'Suggested' ? 'pi-user-plus' : 'pi-exclamation-circle';
  }

  protected followUpBadgeClass(type: FollowUpType): string {
    const base = 'rounded px-2 py-0.5 text-xs font-semibold';
    return type === 'Suggested'
      ? `${base} bg-warning-soft text-warning`
      : `${base} bg-danger-soft text-danger`;
  }

  protected activityIconClass(type: ActivityType): string {
    const base = 'flex size-10 shrink-0 items-center justify-center rounded-full';
    switch (type) {
      case 'visit':
        return `${base} bg-success-soft text-success`;
      case 'prescription':
        return `${base} bg-info-soft text-info`;
      case 'appointment':
        return `${base} bg-primary-soft text-primary`;
      default:
        return `${base} bg-secondary-container text-secondary`;
    }
  }

  protected startVisit(appointmentId: string): void {
    void this.router.navigate(['/patients']);
  }

  protected scheduleFollowUps(): void {
    void this.router.navigate(['/patients']);
  }

  protected viewAllAppointments(): void {
    void this.router.navigate(['/patients']);
  }
}
