import { ChangeDetectionStrategy, Component } from '@angular/core';
import { AvatarModule } from 'primeng/avatar';
import { MenubarModule } from 'primeng/menubar';

@Component({
  selector: 'app-top-header',
  imports: [MenubarModule, AvatarModule],
  templateUrl: './app-top-header.html',
  styleUrl: './app-top-header.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AppTopHeader {}
