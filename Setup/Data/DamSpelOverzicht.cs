using System.ComponentModel.DataAnnotations.Schema;

namespace Setup.Data
{
    public class DamSpelOverzicht
    {
        public int Id { get; set; }
        public List<DamSpel> DamSpellen { get; set; }
        [ForeignKey("CreatorId")]
        public int CreatorId { get; set; }
        public Speler Creator { get; set; }
    }
}
