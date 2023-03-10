using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Setup.Data
{
    public class DamSpel
    {
        public int Id { get; set; }
        [MinLength(4)]
        public string SpelNaam { get; set; }
        [ForeignKey("WinnaarId")]
        public int? WinnaarId { get; set; }
        public Speler? Winnaar { get; set; }
        [ForeignKey("CreatorId")]
        public int CreatorId { get; set; }
        public Speler Creator { get; set; }
        [ForeignKey("DeelnemerId")]
        public int? DeelnemerId { get; set; }
        public Speler? Deelnemer { get; set; }
        [Required]
        public int DamBordId { get; set; }
        public DamBord DamBord { get; set; }
        public bool IsSpelVoorbij { get; set; }


        public DamSpel() { }
        public DamSpel(int id, string spelNaam, int? winnaarId, Speler? winnaar, int creatorId, Speler creator, int? deelnemerId, Speler? deelnemer, int damBordId, DamBord damBord, bool isSpelVoorbij)
        {
            Id = id;
            SpelNaam = spelNaam;
            WinnaarId = winnaarId;
            Winnaar = winnaar;
            CreatorId = creatorId;
            Creator = creator;
            DeelnemerId = deelnemerId;
            Deelnemer = deelnemer;
            DamBordId = damBordId;
            DamBord = damBord;
            IsSpelVoorbij = isSpelVoorbij;
        }
    }
}
