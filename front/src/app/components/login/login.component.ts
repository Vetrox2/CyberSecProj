import { Component, signal, OnInit, inject } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UserService } from '../../services/user.service';
import { UserDto } from '../../models/user.model';
import { LoginLockoutService } from '../../services/login-lockout.service';
import { RecaptchaModule } from 'ng-recaptcha';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RecaptchaModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
})
export class LoginComponent implements OnInit {
  login = signal('');
  password = signal('');
  error = signal('');
  changeButtonEnabled = signal(false);

  readonly captchaKey = '6LeJQQosAAAAAPm6xMxy4lCGFXa8bRvbDk5UCaSz';

  lockoutService = inject(LoginLockoutService);
  private usersService = inject(UserService);

  constructor(private userService: UserService, private router: Router) {}

  ngOnInit() {
    this.lockoutService.checkLockoutStatus();
  }

  async onLogin() {
    this.error.set('');

    if (this.lockoutService.isLocked()) {
      this.setErrorOnLockout();
      return;
    }

    try {
      const user = await this.userService.login({
        login: this.login(),
        password: this.password(),
      });

      this.lockoutService.clearLockout();

      if (user.mustChangePassword || this.isPasswordExpired(user)) {
        this.router.navigate(['/set-password']);
      } else {
        this.router.navigate(['/home']);
      }
    } catch (err: any) {
      console.error('Login failed', err);

      this.lockoutService.recordFailedAttempt();

      if (this.lockoutService.isLocked()) {
        this.setErrorOnLockout();
      } else {
        this.error.set('Login lub hasło niepoprawne');
      }
    }
  }

  isPasswordExpired(user: UserDto): boolean {
    if (!user.passwordValidTo) return false;

    const expiry = new Date(user.passwordValidTo);
    const now = new Date();

    return expiry.getTime() < now.getTime();
  }

  async onCaptchaResolved(captchaResponse: string | null) {
    if (!captchaResponse) {
      this.changeButtonEnabled.set(false);
      return;
    }

    const isValid = await this.usersService.verifyRecaptcha(captchaResponse);
    this.changeButtonEnabled.set(isValid);
  }

  private setErrorOnLockout() {
    const remaining = this.lockoutService.remainingSeconds();
    const formatted = this.lockoutService.formatTime(remaining);
    this.error.set(`Logowanie zablokowane. Spróbuj ponownie za ${formatted}`);
  }
}
