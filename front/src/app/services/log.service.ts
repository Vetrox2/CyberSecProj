import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Log } from '../models/log.model';
import { firstValueFrom } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class LogService {
  private readonly baseUrl = 'https://localhost:7001/api/AuditLogs';
  http = inject(HttpClient);

  getLogs(userId?: string): Promise<Log[]> {
    const url = userId ? `${this.baseUrl}/user/${userId}` : this.baseUrl;
    return firstValueFrom(this.http.get<Log[]>(url));
  }
}
