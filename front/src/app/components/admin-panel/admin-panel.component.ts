import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { UserService } from '../../services/user.service';
import { EditUserDialogComponent } from '../edit-user-dialog/edit-user-dialog.component';
import { UpdateUserDto, UserDto } from '../../models/user.model';
import { CreateUserDialogComponent } from '../create-user-dialog/create-user-dialog.component';
import { SetPasswordDialogComponent } from '../set-password-dialog/set-password-dialog.component';

@Component({
  selector: 'app-admin-panel',
  standalone: true,
  imports: [CommonModule, FormsModule, MatDialogModule],
  templateUrl: './admin-panel.component.html',
  styleUrls: ['./admin-panel.component.scss'],
})
export class AdminPanelComponent implements OnInit {
  private userService = inject(UserService);
  private dialog = inject(MatDialog);

  users = this.userService.users; // computed
  loading = signal(false);

  async ngOnInit(): Promise<void> {
    this.loading.set(true);
    try {
      await this.userService.loadAll();
    } finally {
      this.loading.set(false);
    }
  }

  openCreate() {
    const ref = this.dialog.open(CreateUserDialogComponent, { width: '480px' });
    ref.afterClosed().subscribe((res) => {
      if (res === true) {
        // created — list already updated in service
      }
    });
  }

  openEdit(user: UserDto) {
    const ref = this.dialog.open(EditUserDialogComponent, {
      width: '480px',
      data: { user },
    });
    ref.afterClosed().subscribe();
  }

  openSetPassword(userId: string) {
    const ref = this.dialog.open(SetPasswordDialogComponent, {
      width: '420px',
      data: { id: userId },
    });
    ref.afterClosed().subscribe();
  }

  async toggleBlock(u: UserDto) {
    const dto: UpdateUserDto = { isBlocked: !u.isBlocked };
    await this.userService.update(u.id, dto);
  }

  async toggleRequireRules(u: UserDto) {
    const dto: UpdateUserDto = {
      requirePasswordRules: !u.requirePasswordRules,
    };
    await this.userService.update(u.id, dto);
  }

  async setPasswordValidTo(u: UserDto, isoDate: string | null) {
    const dto: UpdateUserDto = { passwordValidTo: isoDate || null };
    await this.userService.update(u.id, dto);
  }

  async deleteUser(u: UserDto) {
    if (!confirm(`Usuń użytkownika ${u.login}?`)) return;
    await this.userService.delete(u.id);
  }
}
