using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class AppSettings
    {
        [Key]
        public int Id { get; set; }
        public int SessionTimeoutMinutes { get; set; } = 5;
        public int MaxFailedLoginAttempts { get; set; } = 3;
        public int LockoutDurationMinutes { get; set; } = 15;
    }

    public class AppSettingsDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "SessionTimeoutMinutes must be greater than 0")]
        public int SessionTimeoutMinutes { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "MaxFailedLoginAttempts must be greater than 0")]
        public int MaxFailedLoginAttempts { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "LockoutDurationMinutes must be greater than 0")]
        public int LockoutDurationMinutes { get; set; }
    }
}
