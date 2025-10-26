import { Component, inject, OnInit, OnDestroy } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { InactivityService } from './services/inactivity.service';
import { SettingsService } from './services/settings.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})
export class AppComponent implements OnInit, OnDestroy {
  title = 'front';
  private inactivityService = inject(InactivityService);
  private settingsService = inject(SettingsService);

  async ngOnInit(): Promise<void> {
    await this.settingsService.loadSettings();
    this.inactivityService.init();
  }

  ngOnDestroy(): void {
    this.inactivityService.destroy();
  }
}
