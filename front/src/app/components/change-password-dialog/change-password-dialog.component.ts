import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-change-password-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule, MatDialogModule],
  templateUrl: './change-password-dialog.component.html',
  styleUrl: './change-password-dialog.component.scss',
})
export class ChangePasswordDialogComponent {
  private usersService = inject(UserService);
  private dialogRef = inject(MatDialogRef<ChangePasswordDialogComponent>);

  currentPassword = signal('');
  newPassword = signal('');
  repeatPassword = signal('');
  message = signal<string | null>(null);
  loading = signal(false);

  async changePassword() {
    this.message.set(null);

    const current = this.currentPassword().trim();
    const newPass = this.newPassword().trim();
    const repeat = this.repeatPassword().trim();

    if (!current || !newPass || !repeat) {
      this.message.set('Wszystkie pola są wymagane.');
      return;
    }

    if (newPass !== repeat) {
      this.message.set('Nowe hasła nie są takie same.');
      return;
    }

    this.loading.set(true);
    try {
      const dto = {
        currentPassword: current,
        newPassword: newPass,
      };
      const userId = this.usersService.currentUser()?.id;
      if (!userId) {
        this.message.set('Nie znaleziono użytkownika.');
        this.loading.set(false);
        return;
      }
      await this.usersService.changePassword(userId, dto);
      this.dialogRef.close(true);
    } catch (err) {
      this.message.set('Nie udało się zmienić hasła. Sprawdź stare hasło.');
    } finally {
      this.loading.set(false);
    }
  }

  cancel() {
    this.dialogRef.close(false);
  }
}
