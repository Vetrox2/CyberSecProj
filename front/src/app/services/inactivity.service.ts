import { Injectable, inject } from '@angular/core';
import { UserService } from './user.service';
import { SettingsService } from './settings.service';

@Injectable({
  providedIn: 'root',
})
export class InactivityService {
  private userService = inject(UserService);
  private settingsService = inject(SettingsService);

  private inactivityTimeout: any;

  init(): void {
    this.setupInactivityListeners();
    this.resetTimer();
  }

  private setupInactivityListeners(): void {
    const events = [
      'mousedown',
      'mousemove',
      'keypress',
      'scroll',
      'touchstart',
      'click',
    ];

    events.forEach((event) => {
      document.addEventListener(event, () => this.resetTimer(), true);
    });
  }

  private resetTimer(): void {
    this.destroy();

    if (this.userService.currentUser()) {
      const settings = this.settingsService.settings();
      const timeoutMinutes = settings()?.sessionTimeoutMinutes ?? 5;
      const timeoutMs = timeoutMinutes * 60 * 1000;

      this.inactivityTimeout = setTimeout(() => {
        this.logoutDueToInactivity();
      }, timeoutMs);
    }
  }

  private logoutDueToInactivity(): void {
    this.userService.logout();
    alert('Zostałeś wylogowany z powodu braku aktywności');
  }

  destroy(): void {
    if (this.inactivityTimeout) {
      clearTimeout(this.inactivityTimeout);
    }
  }
}
