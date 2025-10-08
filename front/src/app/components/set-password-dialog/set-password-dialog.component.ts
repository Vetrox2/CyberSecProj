import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  MAT_DIALOG_DATA,
  MatDialogRef,
  MatDialogModule,
} from '@angular/material/dialog';
import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-set-password-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule, MatDialogModule],
  templateUrl: './set-password-dialog.component.html',
  styleUrls: ['./set-password-dialog.component.scss'],
})
export class SetPasswordDialogComponent {
  private userService = inject(UserService);
  private dialogRef = inject(MatDialogRef<SetPasswordDialogComponent>);
  private data = inject(MAT_DIALOG_DATA) as { id: string };

  newPassword = signal('');
  loading = signal(false);
  error = signal<string | null>(null);

  async set() {
    if (!this.newPassword().trim()) {
      this.error.set('Hasło wymagane');
      return;
    }
    this.loading.set(true);
    try {
      await this.userService.setPasswordByAdmin(this.data.id, {
        newPassword: this.newPassword(),
      });
      this.dialogRef.close(true);
    } catch (err: any) {
      this.error.set('Błąd ustawienia hasła');
    } finally {
      this.loading.set(false);
    }
  }

  cancel() {
    this.dialogRef.close(false);
  }
}
