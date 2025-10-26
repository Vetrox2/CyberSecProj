using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public class AuditLog
    {
        [Key]
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public DateTime Date { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string ActionType { get; set; } = string.Empty;

        public bool SuccessState { get; set; }
    }

    public static class AuditActionType
    {
        // Authentication
        public const string UserLogin = "User Login";

        // User Management
        public const string UserCreate = "User Created";
        public const string UserEdit = "User Edited";
        public const string UserDelete = "User Deleted";

        // Password Operations
        public const string UserPasswordChange = "Password Changed";
        public const string UserPasswordSetByAdmin = "Password Set by Admin";
        public const string UserPasswordSetByUser = "Password Set by User";
        public const string GenerateOneTimePassword = "One-Time Password Generated";

        // User Queries
        public const string UserGetAll = "All Users Retrieved";
        public const string UserGetById = "User Details Retrieved";

        // Settings Operations
        public const string SettingsGet = "Settings Retrieved";
        public const string SettingsUpdate = "Settings Updated";
    }
}
