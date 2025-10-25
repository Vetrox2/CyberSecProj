using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class OneTimePassword
    {
        [Key]
        public Guid Id { get; set; }

        public string UserLogin { get; set; } = string.Empty;

        public bool Active { get; set; }

        public double Password { get; set; }
    }
}
