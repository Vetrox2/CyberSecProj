using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(100)]
        public string Login { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [MaxLength(200)]
        public string Name { get; set; }

        public bool MustChangePassword { get; set; } = true;

        public bool IsBlocked { get; set; } = false;

        public DateTime? PasswordValidTo { get; set; }

        public bool RequirePasswordRules { get; set; } = true;

        public int RoleId { get; set; }
    }
}
