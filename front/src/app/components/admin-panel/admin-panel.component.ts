import {
  Component,
  OnInit,
  computed,
  inject,
  linkedSignal,
  signal,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { UserService } from '../../services/user.service';
import { SettingsService } from '../../services/settings.service';
import { EditUserDialogComponent } from '../edit-user-dialog/edit-user-dialog.component';
import { UpdateUserDto, UserDto } from '../../models/user.model';
import { CreateUserDialogComponent } from '../create-user-dialog/create-user-dialog.component';
import { SetPasswordDialogComponent } from '../set-password-dialog/set-password-dialog.component';
import { ViewLogsDialogComponent } from '../view-logs-dialog/view-logs-dialog.component';

@Component({
  selector: 'app-admin-panel',
  standalone: true,
  imports: [CommonModule, FormsModule, MatDialogModule],
  templateUrl: './admin-panel.component.html',
  styleUrls: ['./admin-panel.component.scss'],
})
export class AdminPanelComponent implements OnInit {
  private userService = inject(UserService);
  private settingsService = inject(SettingsService);
  private dialog = inject(MatDialog);

  users = this.userService.users;
  loading = signal(false);

  currentSettings = this.settingsService.settings();
  sessionTimeout = linkedSignal(
    () => this.currentSettings()?.sessionTimeoutMinutes ?? 0
  );
  maxFailedAttempts = linkedSignal(
    () => this.currentSettings()?.maxFailedLoginAttempts ?? 0
  );
  lockoutDuration = linkedSignal(
    () => this.currentSettings()?.lockoutDurationMinutes ?? 0
  );
  settingsChanged = signal(false);

  async ngOnInit(): Promise<void> {
    this.loading.set(true);
    try {
      await this.userService.loadAll();
    } finally {
      this.loading.set(false);
    }
  }

  openCreate() {
    const ref = this.dialog.open(CreateUserDialogComponent, { width: '480px' });
    ref.afterClosed().subscribe((res) => {
      if (res === true) {
        // created — list already updated in service
      }
    });
  }

  openLogs(userId?: string) {
    this.dialog.open(ViewLogsDialogComponent, {
      width: '1200px',
      maxWidth: '90vw',
      height: '80vh',
      data: { userId },
    });
  }

  openEdit(user: UserDto) {
    const ref = this.dialog.open(EditUserDialogComponent, {
      width: '480px',
      data: { user },
    });
    ref.afterClosed().subscribe();
  }

  openSetPassword(userId: string) {
    const ref = this.dialog.open(SetPasswordDialogComponent, {
      width: '420px',
      data: { id: userId },
    });
    ref.afterClosed().subscribe();
  }

  async generateOtp(userLogin: string) {
    const otp = await this.userService.generateOneTimePassword(userLogin);
    alert(`Wygenerowane OTP dla ${userLogin}: ${otp}`);
  }

  async toggleBlock(u: UserDto) {
    const dto: UpdateUserDto = { isBlocked: !u.isBlocked };
    await this.userService.update(u.id, dto);
  }

  async toggleRequireRules(u: UserDto) {
    const dto: UpdateUserDto = {
      requirePasswordRules: !u.requirePasswordRules,
    };
    await this.userService.update(u.id, dto);
  }

  async setPasswordValidTo(u: UserDto, isoDate: string | null) {
    const dto: UpdateUserDto = { passwordValidTo: isoDate || null };
    await this.userService.update(u.id, dto);
  }

  async deleteUser(u: UserDto) {
    if (!confirm(`Usuń użytkownika ${u.login}?`)) return;
    await this.userService.delete(u.id);
  }

  // Settings
  incrementSessionTimeout() {
    this.sessionTimeout.update((v) => v + 1);
    this.checkSettingsChanged();
  }

  decrementSessionTimeout() {
    if (this.sessionTimeout() > 1) {
      this.sessionTimeout.update((v) => v - 1);
      this.checkSettingsChanged();
    }
  }

  incrementMaxFailedAttempts() {
    this.maxFailedAttempts.update((v) => v + 1);
    this.checkSettingsChanged();
  }

  decrementMaxFailedAttempts() {
    if (this.maxFailedAttempts() > 1) {
      this.maxFailedAttempts.update((v) => v - 1);
      this.checkSettingsChanged();
    }
  }

  incrementLockoutDuration() {
    this.lockoutDuration.update((v) => v + 1);
    this.checkSettingsChanged();
  }

  decrementLockoutDuration() {
    if (this.lockoutDuration() > 1) {
      this.lockoutDuration.update((v) => v - 1);
      this.checkSettingsChanged();
    }
  }

  private checkSettingsChanged() {
    const current = this.currentSettings();
    if (!current) {
      this.settingsChanged.set(false);
      return;
    }

    const changed =
      this.sessionTimeout() !== current.sessionTimeoutMinutes ||
      this.maxFailedAttempts() !== current.maxFailedLoginAttempts ||
      this.lockoutDuration() !== current.lockoutDurationMinutes;

    this.settingsChanged.set(changed);
  }

  async saveSettings() {
    if (!this.settingsChanged()) return;

    try {
      await this.settingsService.updateAllSettings({
        sessionTimeoutMinutes: this.sessionTimeout(),
        maxFailedLoginAttempts: this.maxFailedAttempts(),
        lockoutDurationMinutes: this.lockoutDuration(),
      });
      this.settingsChanged.set(false);
    } catch (error) {
      alert('Błąd podczas zapisywania ustawień');
    }
  }
}
