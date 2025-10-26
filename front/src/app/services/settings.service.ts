import { HttpClient } from '@angular/common/http';
import { Injectable, signal, computed, inject } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { AppSettingsDto } from '../models/app-settings.model';

@Injectable({
  providedIn: 'root',
})
export class SettingsService {
  private readonly baseUrl = 'https://localhost:7001/api/Settings';
  private readonly http = inject(HttpClient);

  private _settings = signal<AppSettingsDto | null>(null);
  public settings = computed(() => this._settings.asReadonly());

  async loadSettings(): Promise<AppSettingsDto> {
    const settings = await firstValueFrom(
      this.http.get<AppSettingsDto>(this.baseUrl)
    );

    this._settings.set(settings);
    return settings;
  }

  async updateSessionTimeout(minutes: number): Promise<void> {
    const currentSettings = this._settings();
    if (!currentSettings) {
      alert('Nie można zaktualizować ustawień - brak danych');
      return;
    }

    const updatedSettings = await firstValueFrom(
      this.http.put<AppSettingsDto>(this.baseUrl, {
        ...currentSettings,
        sessionTimeoutMinutes: minutes,
      })
    );
    this._settings.set(updatedSettings);
  }

  async updateMaxFailedLoginAttempts(attempts: number): Promise<void> {
    const currentSettings = this._settings();
    if (!currentSettings) {
      alert('Nie można zaktualizować ustawień - brak danych');
      return;
    }

    const updatedSettings = await firstValueFrom(
      this.http.put<AppSettingsDto>(this.baseUrl, {
        ...currentSettings,
        maxFailedLoginAttempts: attempts,
      })
    );
    this._settings.set(updatedSettings);
  }

  async updateLockoutDuration(minutes: number): Promise<void> {
    const currentSettings = this._settings();
    if (!currentSettings) {
      alert('Nie można zaktualizować ustawień - brak danych');
      return;
    }

    const updatedSettings = await firstValueFrom(
      this.http.put<AppSettingsDto>(this.baseUrl, {
        ...currentSettings,
        lockoutDurationMinutes: minutes,
      })
    );
    this._settings.set(updatedSettings);
  }

  async updateAllSettings(settings: AppSettingsDto): Promise<void> {
    const updatedSettings = await firstValueFrom(
      this.http.put<AppSettingsDto>(this.baseUrl, settings)
    );
    this._settings.set(updatedSettings);
  }
}
