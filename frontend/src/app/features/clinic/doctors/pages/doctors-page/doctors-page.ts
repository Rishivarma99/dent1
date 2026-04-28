import { ChangeDetectionStrategy, Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ToastModule } from 'primeng/toast';
import { ConfirmationService, MessageService } from 'primeng/api';
import { DoctorsService, Doctor } from '../../api/doctors-api.service';

@Component({
  selector: 'app-doctors-page',
  imports: [ReactiveFormsModule, ButtonModule, TableModule, DialogModule, InputTextModule, ConfirmDialogModule, ToastModule],
  providers: [ConfirmationService, MessageService],
  templateUrl: './doctors-page.html',
  styleUrl: './doctors-page.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class DoctorsPage implements OnInit {
  private readonly doctorsService = inject(DoctorsService);
  private readonly confirmationService = inject(ConfirmationService);
  private readonly messageService = inject(MessageService);
  private readonly fb = inject(FormBuilder);

  protected readonly doctors = signal<Doctor[]>([]);
  protected readonly loading = signal(false);
  protected readonly dialogVisible = signal(false);
  protected readonly editingDoctor = signal<Doctor | null>(null);
  protected readonly dialogTitle = signal('New Doctor');

  protected readonly form = this.fb.group({
    name: ['', [Validators.required, Validators.minLength(2)]],
    specialty: ['', [Validators.required, Validators.minLength(2)]]
  });

  ngOnInit() {
    this.loadDoctors();
  }

  private loadDoctors() {
    this.loading.set(true);
    this.doctorsService.getAll().subscribe({
      next: (doctors) => {
        this.doctors.set(doctors);
        this.loading.set(false);
      },
      error: () => {
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to load doctors' });
        this.loading.set(false);
      }
    });
  }

  protected openNew() {
    this.editingDoctor.set(null);
    this.dialogTitle.set('New Doctor');
    this.form.reset();
    this.dialogVisible.set(true);
  }

  protected openEdit(doctor: Doctor) {
    this.editingDoctor.set(doctor);
    this.dialogTitle.set('Edit Doctor');
    this.form.patchValue({ name: doctor.name, specialty: doctor.specialty });
    this.dialogVisible.set(true);
  }

  protected saveDoctor() {
    if (this.form.invalid) return;
    const { name, specialty } = this.form.value as { name: string; specialty: string };
    const editing = this.editingDoctor();

    if (editing) {
      this.doctorsService.update(editing.id, { name, specialty }).subscribe({
        next: () => {
          this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Doctor updated' });
          this.dialogVisible.set(false);
          this.loadDoctors();
        },
        error: () => this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to update doctor' })
      });
    } else {
      this.doctorsService.create({ name, specialty }).subscribe({
        next: () => {
          this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Doctor created' });
          this.dialogVisible.set(false);
          this.loadDoctors();
        },
        error: () => this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to create doctor' })
      });
    }
  }

  protected confirmDelete(doctor: Doctor) {
    this.confirmationService.confirm({
      message: `Are you sure you want to delete Dr. ${doctor.name}?`,
      header: 'Confirm Delete',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.doctorsService.delete(doctor.id).subscribe({
          next: () => {
            this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Doctor deleted' });
            this.loadDoctors();
          },
          error: () => this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to delete doctor' })
        });
      }
    });
  }
}
