import {
  Component,
  OnInit,
  Output,
  EventEmitter,
  signal,
  inject,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserService } from '../../services/user.service';
import { ImageCaptchaDto } from '../../models/captcha.model';

@Component({
  selector: 'app-image-captcha',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './image-captcha.component.html',
  styleUrl: './image-captcha.component.scss',
})
export class ImageCaptchaComponent implements OnInit {
  @Output() captchaResolved = new EventEmitter<boolean>();

  captchaData = signal<ImageCaptchaDto | null>(null);
  selectedIndices = signal<Set<number>>(new Set());
  isVerifying = signal(false);
  errorMessage = signal('');

  private userService = inject(UserService);

  async ngOnInit() {
    await this.loadCaptcha();
  }

  async loadCaptcha() {
    try {
      const data = await this.userService.getImageCaptcha();
      this.captchaData.set(data);
      this.selectedIndices.set(new Set());
      this.errorMessage.set('');
    } catch (error) {
      console.error('Failed to load captcha:', error);
      this.errorMessage.set('Failed to load captcha. Please try again.');
    }
  }

  toggleImageSelection(index: number) {
    const currentSelection = new Set(this.selectedIndices());
    if (currentSelection.has(index)) {
      currentSelection.delete(index);
    } else {
      currentSelection.add(index);
    }
    this.selectedIndices.set(currentSelection);
  }

  isImageSelected(index: number): boolean {
    return this.selectedIndices().has(index);
  }

  async verifyCaptcha() {
    const data = this.captchaData();
    if (!data) return;

    this.isVerifying.set(true);
    this.errorMessage.set('');

    try {
      const isValid = await this.userService.verifyImageCaptcha({
        selectedIndices: Array.from(this.selectedIndices()),
        encryptedKey: data.encryptedKey,
      });

      if (isValid) {
        this.captchaResolved.emit(true);
      } else {
        await this.refresh();
        this.errorMessage.set('Incorrect selection. Please try again.');
        this.captchaResolved.emit(false);
      }
    } catch (error) {
      console.error('Captcha verification failed:', error);
      this.errorMessage.set('Verification failed. Please try again.');
      this.captchaResolved.emit(false);
    } finally {
      this.isVerifying.set(false);
    }
  }

  async refresh() {
    await this.loadCaptcha();
    this.captchaResolved.emit(false);
  }
}
