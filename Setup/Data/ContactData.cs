using System.ComponentModel.DataAnnotations;

namespace Setup.Data
{
    public class ContactData
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Naam { get; set; }
        [Required]
        [MaxLength(200)]
        public string Onderwerp { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [Phone]
        public string Phone { get; set; }
        [Required]
        [MaxLength(600)]
        public string Bericht { get; set; }
        [Required]
        public bool Nieuwsbrief { get; set; }
        public DateTime Bellen { get; set; }
    }
}
