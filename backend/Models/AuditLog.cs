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
        public const string UserLogin = "UserLogin";

        // User Management
        public const string UserCreate = "UserCreate";
        public const string UserEdit = "UserEdit";
        public const string UserDelete = "UserDelete";

        // Password Operations
        public const string UserPasswordChange = "UserPasswordChange";
        public const string UserPasswordSetByAdmin = "UserPasswordSetByAdmin";
        public const string UserPasswordSetByUser = "UserPasswordSetByUser";

        // User Queries
        public const string UserGetAll = "UserGetAll";
        public const string UserGetById = "UserGetById";
    }
}
