import { CommonModule, DatePipe, NgFor, NgIf } from '@angular/common';
import { Component, inject } from '@angular/core';
import { LogService } from '../../services/log.service';
import { Log } from '../../models/log.model';
import { MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';

@Component({
  selector: 'app-view-logs-dialog',
  imports: [CommonModule, NgIf, NgFor, DatePipe, MatDialogModule],
  templateUrl: './view-logs-dialog.component.html',
  styleUrl: './view-logs-dialog.component.scss',
})
export class ViewLogsDialogComponent {
  private logService = inject(LogService);
  userId?: string = inject(MAT_DIALOG_DATA).userId;
  loading = false;
  logs: Log[] = [];

  ngOnInit() {
    this.loadLogs();
  }

  private async loadLogs() {
    this.loading = true;
    try {
      this.logs = await this.logService.getLogs(this.userId);
    } finally {
      this.loading = false;
    }
  }
}
