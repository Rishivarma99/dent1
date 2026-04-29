import { ChangeDetectionStrategy, Component, EventEmitter, Output } from '@angular/core';
import { TooltipModule } from 'primeng/tooltip';

@Component({
  selector: 'app-header',
  imports: [TooltipModule],
  templateUrl: './clinic-top-header.html',
  styleUrl: './clinic-top-header.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ClinicTopHeader {
  @Output() readonly openSidebar = new EventEmitter<void>();

  protected readonly userName = 'Dr. Ravi';

  protected onOpenSidebar(): void {
    this.openSidebar.emit();
  }
}
