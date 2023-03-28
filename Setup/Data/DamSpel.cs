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
        [ForeignKey("CreatorId")]
        public int CreatorId { get; set; }
        [ForeignKey("DeelnemerId")]
        public int? DeelnemerId { get; set; }
        [Required]
        public int DamBordId { get; set; }
        public bool IsSpelVoorbij { get; set; }


        public DamSpel() { }
        public DamSpel(int id, string spelNaam, int? winnaarId, int creatorId, int? deelnemerId, int damBordId, bool isSpelVoorbij)
        {
            Id = id;
            SpelNaam = spelNaam;
            WinnaarId = winnaarId;
            CreatorId = creatorId;
            DeelnemerId = deelnemerId;
            DamBordId = damBordId;
            IsSpelVoorbij = isSpelVoorbij;
        }
    }
}
