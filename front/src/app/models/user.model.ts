export interface UserDto {
  id: string;
  login: string;
  name?: string | null;
  mustChangePassword: boolean;
  isBlocked: boolean;
  roleId: number;
  passwordValidTo?: string | null;
  requirePasswordRules: boolean;
}

export interface CreateUserDto {
  login: string;
  password: string;
  name?: string | null;
  roleId?: number;
  requirePasswordRules?: boolean;
  passwordValidTo?: string | null;
}

export interface UpdateUserDto {
  name?: string | null;
  isBlocked?: boolean | null;
  roleId?: number | null;
  requirePasswordRules?: boolean | null;
  passwordValidTo?: string | null;
}

export interface ChangePasswordDto {
  currentPassword: string;
  newPassword: string;
}

export interface SetNewPasswordDto {
  newPassword: string;
}

export interface LoginDto {
  login: string;
  password: string;
}
