import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';

@Component({
  selector: 'app-dashboard-page',
  imports: [ButtonModule, CardModule],
  templateUrl: './dashboard-page.html',
  styleUrl: './dashboard-page.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class DashboardPage {
  private readonly router = inject(Router);

  protected goToDoctors(): void {
    void this.router.navigate(['/doctors']);
  }

  protected goToPatients(): void {
    void this.router.navigate(['/patients']);
  }

  protected goToLogin(): void {
    void this.router.navigate(['/auth/login']);
  }
}
