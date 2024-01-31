using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Setup.Data
{
    public class DamSpel
    {
        public int Id { get; set; }
        [MinLength(4)]
        public string SpelNaam { get; set; }
        [ForeignKey("Winnaar")]
        public string? Winnaar { get; set; }
        [ForeignKey("Creator")]
        public string Creator { get; set; }
        [ForeignKey("Deelnemer")]
        public string? Deelnemer { get; set; }
        [Required]
        public int DamBordId { get; set; }
        public bool IsSpelVoorbij { get; set; }


        public DamSpel() { }
        public DamSpel(int id, string spelNaam, string? winnaar, string creator, string? deelnemer, int damBordId, bool isSpelVoorbij)
        {
            Id = id;
            SpelNaam = spelNaam;
            Winnaar = winnaar;
            Creator = creator;
            Deelnemer = deelnemer;
            DamBordId = damBordId;
            IsSpelVoorbij = isSpelVoorbij;
        }
    }
}
