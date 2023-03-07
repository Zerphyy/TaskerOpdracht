using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Setup.Data
{
    public class Speler
    {
        public int ID { get; set; }
        [Required]
        [MinLength(4)]
        public string Naam { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [PasswordPropertyText]
        public string Wachtwoord { get; set; }
    }
}
