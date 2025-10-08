import { Component, signal } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UserService } from '../../services/user.service';
import { UserDto } from '../../models/user.model';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
})
export class LoginComponent {
  login = signal('');
  password = signal('');
  error = signal('');

  constructor(private userService: UserService, private router: Router) {}

  async onLogin() {
    this.error.set('');
    try {
      const user = await this.userService.login({
        login: this.login(),
        password: this.password(),
      });

      if (user.mustChangePassword || this.isPasswordExpired(user)) {
        this.router.navigate(['/set-password']);
      } else {
        this.router.navigate(['/home']);
      }
    } catch (err: any) {
      console.error('Login failed', err);
      this.error.set('Login lub hasło niepoprawne');
    }
  }

  isPasswordExpired(user: UserDto): boolean {
    if (!user.passwordValidTo) return false;

    const expiry = new Date(user.passwordValidTo);
    const now = new Date();

    return expiry.getTime() < now.getTime();
  }
}
