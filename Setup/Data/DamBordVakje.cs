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
        public DamStuk DamStuk { get; set; }
        public DamBordVakje()
        {
        }
        public DamBordVakje(int row, int col, DamStuk stuk)
        {
            this.Row = row;
            this.Col = col;
            this.DamStuk = stuk;
        }
    }

}
