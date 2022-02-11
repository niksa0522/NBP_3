using System.ComponentModel.DataAnnotations;

namespace NBP_3.Models
{
    public class User
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
