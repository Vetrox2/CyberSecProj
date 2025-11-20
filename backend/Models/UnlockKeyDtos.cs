using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class GenerateKeyDto
    {
        [Required]
        public Guid UserId { get; set; }
    }

    public class UnlockKeyDto
    {
        public string EncryptedKey { get; set; } = string.Empty;
    }

    public class ValidateUnlockKeyDto
    {
        [Required]
        public string EncryptedKey { get; set; } = string.Empty;
    }

    public class UnlockKeyResponseDto
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
    }
}
