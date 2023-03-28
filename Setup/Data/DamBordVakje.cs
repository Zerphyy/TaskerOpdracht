using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Setup.Data
{
    public class DamBordVakje
    {
        public int Id { get; set; }
        [Required]
        public int Row { get; set; }
        [Required]
        public int Col { get; set; }
        [ForeignKey("DamStuk")]
        public int? DamStukId { get; set; }
        [ForeignKey("DamBord")]
        public int DamBordId { get; set; }
        public DamBordVakje()
        {
        }
        public DamBordVakje(int id, int row, int col)
        {
            this.Row = row;
            this.Col = col;
        }
    }

}
