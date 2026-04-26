import { ChangeDetectionStrategy, Component, signal } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { InputTextModule } from 'primeng/inputtext';
import { TagModule } from 'primeng/tag';
import { CardModule } from 'primeng/card';
import { AppSidebarNav } from '../../../../shared/layout/components/app-sidebar-nav/app-sidebar-nav';
import { AppTopHeader } from '../../../../shared/layout/components/app-top-header/app-top-header';

type PatientStatus = 'ACTIVE' | 'IN TREATMENT' | 'NEW';

interface PatientRecord {
  initials: string;
  name: string;
  id: string;
  phone: string;
  email: string;
  ageGender: string;
  lastVisit: string;
  nextAppointment: string;
  status: PatientStatus;
}

@Component({
  selector: 'app-patients-page',
  imports: [ButtonModule, TableModule, InputTextModule, TagModule, CardModule, AppSidebarNav, AppTopHeader],
  templateUrl: './patients-page.html',
  styleUrl: './patients-page.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class PatientsPage {
  protected readonly searchTerm = signal('');

  private readonly patientRecords: PatientRecord[] = [
    {
      initials: 'MT',
      name: 'Michael Thompson',
      id: '#0001',
      phone: '+1 (555) 123-4567',
      email: 'michael.t@email.com',
      ageGender: '45 - Male',
      lastVisit: '2026-03-28',
      nextAppointment: '2026-04-10',
      status: 'ACTIVE'
    },
    {
      initials: 'SM',
      name: 'Sarah Martinez',
      id: '#0002',
      phone: '+1 (555) 234-5678',
      email: 'sarah.m@email.com',
      ageGender: '32 - Female',
      lastVisit: '2026-04-01',
      nextAppointment: 'No Appointment',
      status: 'ACTIVE'
    },
    {
      initials: 'JW',
      name: 'James Wilson',
      id: '#0003',
      phone: '+1 (555) 345-6789',
      email: 'james.w@email.com',
      ageGender: '28 - Male',
      lastVisit: '2026-03-15',
      nextAppointment: '2026-04-05',
      status: 'IN TREATMENT'
    },
    {
      initials: 'ED',
      name: 'Emily Davis',
      id: '#0004',
      phone: '+1 (555) 456-7890',
      email: 'emily.d@email.com',
      ageGender: '38 - Female',
      lastVisit: 'Never',
      nextAppointment: 'Today',
      status: 'NEW'
    }
  ];

  protected readonly patients = signal(this.patientRecords);
  protected readonly patientRequests = signal([
    'Appointment Request - from John Doe - 2h ago',
    'Record Transfer - from Mary Smith - 5h ago',
    'Prescription Refill - from Robert Brown - 1 day ago'
  ]);
  protected readonly recentPatients = signal([
    'Michael Vance - Updated Medical History - 2h ago',
    'Julianne Smith - Lab Results Uploaded - 4h ago'
  ]);

  protected onSearch(value: string): void {
    const normalizedValue = value.trim().toLowerCase();
    this.searchTerm.set(value);
    if (!normalizedValue) {
      this.patients.set(this.patientRecords);
      return;
    }

    this.patients.set(
      this.patientRecords.filter(
        (patient) =>
          patient.name.toLowerCase().includes(normalizedValue) ||
          patient.phone.toLowerCase().includes(normalizedValue) ||
          patient.email.toLowerCase().includes(normalizedValue)
      )
    );
  }

  protected statusSeverity(status: PatientStatus): 'success' | 'warn' | 'info' {
    if (status === 'ACTIVE') {
      return 'success';
    }

    if (status === 'IN TREATMENT') {
      return 'warn';
    }

    return 'info';
  }
}
