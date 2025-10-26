import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { UserService } from '../../services/user.service';
import { RoleService } from '../../services/role.service';

@Component({
  selector: 'app-create-user-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule, MatDialogModule],
  templateUrl: './create-user-dialog.component.html',
  styleUrls: ['./create-user-dialog.component.scss'],
})
export class CreateUserDialogComponent {
  private userService = inject(UserService);
  private roleService = inject(RoleService);
  private dialogRef = inject(MatDialogRef<CreateUserDialogComponent>);

  roles = this.roleService.roles();

  login = signal('');
  otp = signal('');
  password = signal('');
  name = signal('');
  selectedRoleId = signal<number | null>(null);
  requirePasswordRules = signal(false);
  passwordValidTo = signal<string | null>(null);
  loading = signal(false);
  error = signal<string | null>(null);

  async create() {
    this.error.set(null);
    if (this.otp()) {
      const random = Math.random().toString(36).slice(2, 10);
      this.password.set(random.toString());
    }
    if (!this.login().trim() || !this.password().trim()) {
      this.error.set('Login i hasło są wymagane');
      return;
    }
    this.loading.set(true);
    try {
      await this.userService.create({
        login: this.login(),
        password: this.password(),
        name: this.name() || undefined,
        roleId: this.selectedRoleId() || undefined,
        requirePasswordRules: this.requirePasswordRules(),
        passwordValidTo: this.passwordValidTo() || undefined,
      });
      this.dialogRef.close(true);
    } catch (err: any) {
      this.error.set(err?.message ?? 'Błąd tworzenia');
    } finally {
      this.loading.set(false);
    }
  }

  onLoginChange(newLogin: string) {
    this.login.set(newLogin);
    this.otp.set('');
  }

  async generateOtp() {
    const otp = await this.userService.generateOneTimePassword(this.login());
    this.otp.set(otp.toString());
  }

  cancel() {
    this.dialogRef.close(false);
  }
}
