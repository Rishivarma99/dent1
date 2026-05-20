import { ChangeDetectionStrategy, Component, inject, input, model, OnInit, output } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { DrawerModule } from 'primeng/drawer';
import { InputTextModule } from 'primeng/inputtext';
import { SelectModule } from 'primeng/select';
import { TextareaModule } from 'primeng/textarea';

export type AppointmentType = 'new' | 'follow-up';

export interface DayAppointmentSlot {
  time: string;
  label: string;
  variant: 'default' | 'current' | 'available';
}

@Component({
  selector: 'app-create-appointment-drawer',
  imports: [
    ReactiveFormsModule,
    DrawerModule,
    ButtonModule,
    InputTextModule,
    SelectModule,
    TextareaModule
  ],
  templateUrl: './create-appointment-drawer.html',
  styleUrl: './create-appointment-drawer.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CreateAppointmentDrawer implements OnInit {
  private readonly fb = inject(FormBuilder);

  readonly visible = model(false);
  readonly bookingPatientName = input('John Smith');

  readonly closed = output<void>();
  readonly submitted = output<void>();

  protected readonly doctorOptions = [
    { label: 'Dr. Sarah Chen', value: 'Dr. Sarah Chen' },
    { label: 'Dr. Marcus Thorne', value: 'Dr. Marcus Thorne' },
    { label: 'Dr. Elena Rodriguez', value: 'Dr. Elena Rodriguez' }
  ];

  protected readonly treatmentOptions = [
    { label: 'Root Canal (Oct 15)', value: 'Root Canal (Oct 15)' },
    { label: 'Cleaning (Sep 02)', value: 'Cleaning (Sep 02)' },
    { label: 'Skin Therapy (Jul 20)', value: 'Skin Therapy (Jul 20)' }
  ];

  protected readonly selectedPatient = {
    name: 'Michael Vance',
    meta: 'Male • 45Y • +1 (555) 123-4567'
  };

  protected readonly dayAppointments: readonly DayAppointmentSlot[] = [
    { time: '10:00', label: 'Ravi Kumar', variant: 'default' },
    { time: '10:30', label: 'Sita Sharma', variant: 'default' },
    { time: '11:00', label: 'Current Booking', variant: 'current' },
    { time: '11:30', label: 'Available Slot', variant: 'available' }
  ];

  protected readonly form = this.fb.group({
    patientSearch: [''],
    doctor: ['Dr. Sarah Chen', Validators.required],
    date: [this.todayIso(), Validators.required],
    arrivalTime: ['11:00', Validators.required],
    appointmentType: ['follow-up' as AppointmentType, Validators.required],
    linkedTreatment: ['', Validators.required],
    notes: ['']
  });

  ngOnInit(): void {
    this.setAppointmentType('follow-up');
  }

  protected get appointmentType(): AppointmentType {
    return this.form.controls.appointmentType.value ?? 'follow-up';
  }

  protected setAppointmentType(type: AppointmentType): void {
    this.form.controls.appointmentType.setValue(type);
    if (type === 'new') {
      this.form.controls.linkedTreatment.clearValidators();
    } else {
      this.form.controls.linkedTreatment.setValidators(Validators.required);
    }
    this.form.controls.linkedTreatment.updateValueAndValidity();
  }

  protected onHide(): void {
    this.closed.emit();
  }

  protected close(): void {
    this.visible.set(false);
    this.closed.emit();
  }

  protected submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.submitted.emit();
    this.visible.set(false);
  }

  private todayIso(): string {
    const today = new Date();
    const month = String(today.getMonth() + 1).padStart(2, '0');
    const day = String(today.getDate()).padStart(2, '0');
    return `${today.getFullYear()}-${month}-${day}`;
  }
}
