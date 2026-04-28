import { ChangeDetectionStrategy, Component, EventEmitter, Output } from '@angular/core';

@Component({
  selector: 'app-header',
  templateUrl: './clinic-top-header.html',
  styleUrl: './clinic-top-header.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ClinicTopHeader {
  @Output() readonly openSidebar = new EventEmitter<void>();

  protected onOpenSidebar(): void {
    this.openSidebar.emit();
  }
}
