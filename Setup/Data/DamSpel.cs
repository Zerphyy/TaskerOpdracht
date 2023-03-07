using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Setup.Data
{
    public class DamSpel
    {
        public int Id { get; set; }
        public List<Speler> Spelers { get; set; }
        [ForeignKey("WinnaarId")]
        public int? WinnaarId { get; set; }
        public Speler? Winnaar { get; set; }
        [Required]
        public int DamBordId { get; set; }
        public DamBord DamBord { get; set; }
        public bool IsSpelVoorbij { get; set; }
    }
}
