using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public class LogDto
    {
        public string UserLogin { get; set; } = string.Empty;

        public string Timestamp { get; set; } = string.Empty;

        public string Action { get; set; } = string.Empty;

        public bool Success { get; set; }
    }
}
