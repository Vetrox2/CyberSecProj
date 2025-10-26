import { HttpClient } from '@angular/common/http';
import { Injectable, signal, computed, inject } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { RoleDto } from '../models/role.model';

@Injectable({
  providedIn: 'root',
})
export class RoleService {
  private readonly baseUrl = 'https://localhost:7001/api/Roles';
  private readonly http = inject(HttpClient);

  private _roles = signal<RoleDto[]>([]);
  public roles = computed(() => this._roles.asReadonly());

  async loadAll(): Promise<RoleDto[]> {
    const roles = await firstValueFrom(this.http.get<RoleDto[]>(this.baseUrl));
    this._roles.set(roles);
    return roles;
  }
}
