import { ChangeDetectionStrategy, Component } from '@angular/core';

@Component({
  selector: 'app-appointments-page',
  template: `
    <section class="flex flex-col gap-4 bg-background p-4 text-text sm:p-6 lg:p-8">
      <header>
        <h1 class="text-2xl font-bold text-text-strong sm:text-3xl">Appointments</h1>
        <p class="mt-1 text-sm text-text-subtle">Scheduling and calendar — coming soon</p>
      </header>
    </section>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AppointmentsPage {}
