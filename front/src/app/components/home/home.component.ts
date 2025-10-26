import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { UserService } from '../../services/user.service';
import { ChangePasswordDialogComponent } from '../change-password-dialog/change-password-dialog.component';
import { AdminPanelComponent } from '../admin-panel/admin-panel.component';
import { RoleService } from '../../services/role.service';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, MatDialogModule, AdminPanelComponent],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
})
export class HomeComponent implements OnInit {
  private userService = inject(UserService);
  private router = inject(Router);
  private dialog = inject(MatDialog);
  private roleService = inject(RoleService);

  // currentUser comes from service computed, but for now you said you'll supply it;
  // we will read from service.currentUser computed.
  currentUser = this.userService.currentUser;

  // helper local signal for loading state
  loading = signal(false);

  async ngOnInit() {
    await this.roleService.loadAll();
  }

  openChangePassword() {
    this.dialog
      .open(ChangePasswordDialogComponent, {
        width: '420px',
        data: { userId: this.currentUser()?.id ?? '' }, // if null, will be empty string
      })
      .afterClosed()
      .subscribe((res) => {
        if (res === true) {
          // optionally do something on success
        }
      });
  }

  logout() {
    // call service logout if exists (see snippet above); otherwise just clear and navigate
    if ((this.userService as any).logout) {
      (this.userService as any).logout();
    } else {
      // fallback: try to set null via direct hack (not ideal)
      // (this.usersService as any)._currentUser?.set(null);
    }
    this.router.navigateByUrl('/'); // or to login route
  }
}
