using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class Action
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        public bool IsActivated { get; set; } = false;

        public int Counter { get; set; } = 0;

        public User? User { get; set; }
    }
}
