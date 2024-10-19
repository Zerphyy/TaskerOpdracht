using System.ComponentModel.DataAnnotations;

namespace Setup.Data
{
    public class Gebruiker
    {
        [Required]
        [EmailAddress]
        [Key]
        public string Email { get; set; }
        [Required]
        [MinLength(4)]
        public string Naam { get; set; }
        
        [Required]
        //hashed
        public string Wachtwoord { get; set; }
        public string Rol { get; set; }

        public Gebruiker() { }
        public Gebruiker(string naam, string email, string wachtwoord)
        {
            this.Email = email;
            this.Naam = naam;
            this.Wachtwoord = wachtwoord;
            Rol = "Gebruiker";
        }
    }
}
