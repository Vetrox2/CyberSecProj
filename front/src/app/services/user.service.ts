import { HttpClient } from '@angular/common/http';
import { computed, Injectable, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import {
  UserDto,
  CreateUserDto,
  UpdateUserDto,
  ChangePasswordDto,
  SetNewPasswordDto,
  LoginDto,
} from '../models/user.model';
import { ImageCaptchaDto, VerifyCaptchaDto } from '../models/captcha.model';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private readonly baseUrl = 'https://localhost:7001/api/Users';

  private _users = signal<UserDto[]>([]);
  private _currentUser = signal<UserDto | null>(null);
  public users = computed(() => this._users());
  public currentUser = computed(() => this._currentUser());

  constructor(private http: HttpClient, private router: Router) {}

  async loadAll(): Promise<UserDto[]> {
    const obs = this.http.get<UserDto[]>(this.baseUrl);
    const data = await firstValueFrom(obs);
    this._users.set(data);
    return data;
  }

  getById(id: string) {
    return this.http.get<UserDto>(`${this.baseUrl}/${id}`);
  }

  async create(dto: CreateUserDto): Promise<UserDto> {
    const created = await firstValueFrom(
      this.http.post<UserDto>(this.baseUrl, dto)
    );
    this._users.update((list) => [...list, created]);
    return created;
  }

  async update(id: string, dto: UpdateUserDto): Promise<void> {
    await firstValueFrom(this.http.put<void>(`${this.baseUrl}/${id}`, dto));
    this._users.update((list) =>
      list.map((u) => (u.id === id ? ({ ...u, ...dto } as UserDto) : u))
    );
  }

  async delete(id: string): Promise<void> {
    await firstValueFrom(this.http.delete<void>(`${this.baseUrl}/${id}`));
    this._users.update((list) => list.filter((u) => u.id !== id));
  }

  async changePassword(id: string, dto: ChangePasswordDto): Promise<void> {
    await firstValueFrom(
      this.http.post<void>(`${this.baseUrl}/${id}/change-password`, dto)
    );
    const updated = await firstValueFrom(
      this.http.get<UserDto>(`${this.baseUrl}/${id}`)
    );
    this._users.update((list) => list.map((u) => (u.id === id ? updated : u)));
  }

  async setPasswordByAdmin(id: string, dto: SetNewPasswordDto): Promise<void> {
    await firstValueFrom(
      this.http.post<void>(`${this.baseUrl}/${id}/set-password-admin`, dto)
    );
    const updated = await firstValueFrom(
      this.http.get<UserDto>(`${this.baseUrl}/${id}`)
    );
    this._users.update((list) => list.map((u) => (u.id === id ? updated : u)));
  }

  async setPasswordByUser(id: string, dto: SetNewPasswordDto): Promise<void> {
    await firstValueFrom(
      this.http.post<void>(`${this.baseUrl}/${id}/set-password-user`, dto)
    );
    const updated = await firstValueFrom(
      this.http.get<UserDto>(`${this.baseUrl}/${id}`)
    );
    this._users.update((list) => list.map((u) => (u.id === id ? updated : u)));
  }

  async login(loginDto: LoginDto): Promise<UserDto> {
    const user = await firstValueFrom(
      this.http.post<UserDto>(`${this.baseUrl}/login`, loginDto)
    );
    this._currentUser.set(user);
    return user;
  }

  logout(): void {
    this._currentUser.set(null);
    this.router.navigate(['']);
  }

  async generateOneTimePassword(userLogin: string): Promise<number> {
    return await firstValueFrom(
      this.http.post<number>(`${this.baseUrl}/otp/${userLogin}`, {})
    );
  }

  async verifyRecaptcha(token: string): Promise<boolean> {
    const response = await firstValueFrom(
      this.http.post<boolean>(`${this.baseUrl}/recaptcha/${token}`, {})
    );
    return response;
  }

  async getImageCaptcha(): Promise<ImageCaptchaDto> {
    return await firstValueFrom(
      this.http.get<ImageCaptchaDto>(`${this.baseUrl}/image-captcha`)
    );
  }

  async verifyImageCaptcha(dto: VerifyCaptchaDto): Promise<boolean> {
    const result = await firstValueFrom(
      this.http.post<boolean>(`${this.baseUrl}/verify-image-captcha`, dto)
    );
    return result;
  }
}
