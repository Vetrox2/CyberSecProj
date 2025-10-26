import { Injectable, signal, computed, inject } from '@angular/core';
import { SettingsService } from './settings.service';

interface LockoutInfo {
  failedAttempts: number;
  lockedUntil: string | null;
}

@Injectable({
  providedIn: 'root',
})
export class LoginLockoutService {
  private readonly STORAGE_KEY = 'login_lockout';
  private settingsService = inject(SettingsService);

  private _remainingSeconds = signal(0);
  private _isLocked = signal(false);

  public remainingSeconds = computed(() => this._remainingSeconds());
  public isLocked = computed(() => this._isLocked());

  private countdownInterval?: ReturnType<typeof setInterval>;

  constructor() {
    this.checkLockoutStatus();
  }

  checkLockoutStatus(): void {
    const lockoutInfo = this.getLockoutInfo();

    if (lockoutInfo.lockedUntil) {
      const lockedUntilDate = new Date(lockoutInfo.lockedUntil);
      const now = new Date();

      if (lockedUntilDate > now) {
        const remainingMs = lockedUntilDate.getTime() - now.getTime();
        const remainingSecs = Math.ceil(remainingMs / 1000);

        this._isLocked.set(true);
        this._remainingSeconds.set(remainingSecs);
        this.startCountdown();
      } else {
        this.clearLockout();
      }
    }
  }

  recordFailedAttempt(): void {
    const settings = this.settingsService.settings()();
    const maxAttempts = settings?.maxFailedLoginAttempts ?? 3;
    const lockoutMinutes = settings?.lockoutDurationMinutes ?? 15;

    const lockoutInfo = this.getLockoutInfo();
    lockoutInfo.failedAttempts++;

    if (lockoutInfo.failedAttempts >= maxAttempts) {
      const lockoutDate = new Date();
      lockoutDate.setMinutes(lockoutDate.getMinutes() + lockoutMinutes);

      lockoutInfo.lockedUntil = lockoutDate.toISOString();

      this._isLocked.set(true);
      const remainingSecs = lockoutMinutes * 60;
      this._remainingSeconds.set(remainingSecs);

      this.startCountdown();
    }

    this.saveLockoutInfo(lockoutInfo);
  }

  clearLockout(): void {
    this.stopCountdown();
    localStorage.removeItem(this.STORAGE_KEY);
    this._isLocked.set(false);
    this._remainingSeconds.set(0);
  }

  formatTime(seconds: number): string {
    const mins = Math.floor(seconds / 60);
    const secs = seconds % 60;
    return `${mins}:${secs.toString().padStart(2, '0')}`;
  }

  private startCountdown(): void {
    this.stopCountdown();

    this.countdownInterval = setInterval(() => {
      const current = this._remainingSeconds();

      if (current <= 1) {
        this.clearLockout();
      } else {
        this._remainingSeconds.set(current - 1);
      }
    }, 1000);
  }

  private stopCountdown(): void {
    if (this.countdownInterval) {
      clearInterval(this.countdownInterval);
      this.countdownInterval = undefined;
    }
  }

  private getLockoutInfo(): LockoutInfo {
    const stored = localStorage.getItem(this.STORAGE_KEY);

    if (stored) {
      try {
        return JSON.parse(stored) as LockoutInfo;
      } catch {
        return { failedAttempts: 0, lockedUntil: null };
      }
    }

    return { failedAttempts: 0, lockedUntil: null };
  }

  private saveLockoutInfo(info: LockoutInfo): void {
    localStorage.setItem(this.STORAGE_KEY, JSON.stringify(info));
  }

  ngOnDestroy(): void {
    this.stopCountdown();
  }
}
