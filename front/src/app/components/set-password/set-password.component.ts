import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-set-password',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './set-password.component.html',
  styleUrl: './set-password.component.scss',
})
export class SetPasswordComponent {
  private usersService = inject(UserService);
  private router = inject(Router);

  newPassword = signal('');
  repeatPassword = signal('');
  message = signal<string | null>(null);
  loading = signal(false);

  async changePassword() {
    const newPass = this.newPassword();
    const repeatPass = this.repeatPassword();

    if (newPass.trim() === '' || repeatPass.trim() === '') {
      this.message.set('Uzupełnij oba pola hasła.');
      return;
    }

    if (newPass !== repeatPass) {
      this.message.set('Hasła nie są takie same.');
      return;
    }

    this.loading.set(true);
    this.message.set(null);

    try {
      const userId = this.usersService.currentUser()?.id;
      if (!userId) {
        this.message.set('Nie znaleziono użytkownika.');
        this.loading.set(false);
        return;
      }

      await this.usersService.setPasswordByUser(userId, {
        newPassword: newPass,
      });
      this.message.set('Hasło zostało pomyślnie zmienione.');

      setTimeout(() => this.router.navigate(['/home']), 1000);
    } catch (err) {
      this.message.set('Nie udało się zmienić hasła. Spróbuj ponownie.');
    } finally {
      this.loading.set(false);
    }
  }
}
