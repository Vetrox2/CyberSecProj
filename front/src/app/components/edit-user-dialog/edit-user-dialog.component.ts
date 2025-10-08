import { Component, inject, Signal, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  MAT_DIALOG_DATA,
  MatDialogRef,
  MatDialogModule,
} from '@angular/material/dialog';
import { UserService } from '../../services/user.service';
import { UserDto } from '../../models/user.model';

@Component({
  selector: 'app-edit-user-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule, MatDialogModule],
  templateUrl: './edit-user-dialog.component.html',
  styleUrls: ['./edit-user-dialog.component.scss'],
})
export class EditUserDialogComponent {
  private userService = inject(UserService);
  private dialogRef = inject(MatDialogRef<EditUserDialogComponent>);
  private data = inject(MAT_DIALOG_DATA) as { user: UserDto };

  loading = signal(true);
  error = signal<string | null>(null);

  name = signal('');
  isAdmin = signal(false);
  isBlocked = signal(false);
  requirePasswordRules = signal(false);
  passwordValidTo = signal<string | null>(null);

  constructor() {
    this.load();
  }

  load() {
    try {
      const user = this.data.user;
      this.name.set(user.name ?? '');
      this.isAdmin.set(user.isAdmin);
      this.isBlocked.set(user.isBlocked);
      this.requirePasswordRules.set(user.requirePasswordRules);
      this.passwordValidTo.set(user.passwordValidTo ?? null);
    } catch (err: any) {
      this.error.set('Nie udało się załadować użytkownika');
    } finally {
      this.loading.set(false);
    }
  }

  async save() {
    this.error.set(null);
    const dto = {
      name: this.name(),
      isAdmin: this.isAdmin(),
      isBlocked: this.isBlocked(),
      requirePasswordRules: this.requirePasswordRules(),
      passwordValidTo: this.passwordValidTo() || null,
    };
    try {
      const user = this.data.user;
      await this.userService.update(user.id, dto);
      this.dialogRef.close(true);
    } catch (err: any) {
      this.error.set('Błąd zapisu');
    }
  }

  cancel() {
    this.dialogRef.close(false);
  }
}
