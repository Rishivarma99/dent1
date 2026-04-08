import { ChangeDetectionStrategy, Component, computed, inject, OnInit, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { ToolbarModule } from 'primeng/toolbar';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';
import { PatientsService, Patient } from '../../sevices/patients.service';

@Component({
  selector: 'app-patients-page',
  imports: [
    DatePipe,
    ReactiveFormsModule,
    ButtonModule,
    TableModule,
    DialogModule,
    InputTextModule,
    ToolbarModule,
    ToastModule,
  ],
  providers: [MessageService],
  templateUrl: './patients-page.html',
  styleUrl: './patients-page.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PatientsPage implements OnInit {
  private readonly patientsService = inject(PatientsService);
  private readonly messageService = inject(MessageService);
  private readonly fb = inject(FormBuilder);
  private readonly router = inject(Router);

  protected readonly patients = signal<Patient[]>([]);
  protected readonly loading = signal(false);
  protected readonly dialogVisible = signal(false);
  protected readonly searchPhone = signal('');

  protected readonly visiblePatients = computed(() => {
    const search = this.searchPhone().trim();
    if (!search) return this.patients();
    return this.patients().filter((p) => p.phone.toLowerCase().includes(search.toLowerCase()));
  });

  protected readonly form = this.fb.group({
    name: ['', [Validators.required, Validators.minLength(2)]],
    phone: ['', [Validators.required, Validators.minLength(5)]],
  });

  ngOnInit() {
    this.loadPatients();
  }

  protected loadPatients() {
    this.loading.set(true);
    this.patientsService.getAll().subscribe({
      next: (patients) => {
        this.patients.set(patients);
        this.loading.set(false);
      },
      error: () => {
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to load patients' });
        this.loading.set(false);
      },
    });
  }

  protected openNew() {
    this.form.reset();
    this.dialogVisible.set(true);
  }

  protected createPatient() {
    if (this.form.invalid) return;
    const { name, phone } = this.form.value as { name: string; phone: string };

    this.patientsService.create({ name, phone }).subscribe({
      next: () => {
        this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Patient created' });
        this.dialogVisible.set(false);
        this.loadPatients();
      },
      error: () =>
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to create patient' }),
    });
  }

  protected onSearch(value: string) {
    this.searchPhone.set(value);
  }

  protected searchServer() {
    const phone = this.searchPhone().trim();
    if (!phone) {
      this.loadPatients();
      return;
    }

    this.loading.set(true);
    this.patientsService.searchByPhone(phone).subscribe({
      next: (patients) => {
        this.patients.set(patients);
        this.loading.set(false);
      },
      error: () => {
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Phone search failed' });
        this.loading.set(false);
      },
    });
  }

  protected goHome() {
    void this.router.navigate(['/home']);
  }
}
