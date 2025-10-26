using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Login { get; set; }
        public string Name { get; set; }
        public bool MustChangePassword { get; set; }
        public bool IsBlocked { get; set; }
        public DateTime? PasswordValidTo { get; set; }
        public bool RequirePasswordRules { get; set; }
        public int RoleId { get; set; }
    }

    public class CreateUserDto
    {
        [Required]
        [MaxLength(100)]
        public string Login { get; set; }

        [Required]
        public string Password { get; set; }

        [MaxLength(200)]
        public string Name { get; set; }

        public int? RoleId { get; set; }
        public bool RequirePasswordRules { get; set; } = false;
        public DateTime? PasswordValidTo { get; set; }
    }

    public class UpdateUserDto
    {
        [MaxLength(200)]
        public string? Name { get; set; }

        public bool? IsBlocked { get; set; }

        public int? RoleId { get; set; }

        public bool? RequirePasswordRules { get; set; }

        public DateTime? PasswordValidTo { get; set; }
    }

    public class ChangePasswordDto
    {
        [Required]
        public string CurrentPassword { get; set; }

        [Required]
        public string NewPassword { get; set; }
    }

    public class SetNewPasswordDto
    {
        [Required]
        public string NewPassword { get; set; }
    }

    public class LoginDto
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }
}
