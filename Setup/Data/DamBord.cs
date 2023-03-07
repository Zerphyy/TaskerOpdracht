using System.ComponentModel.DataAnnotations;

namespace Setup.Data
{
    public class DamBord
    {
        public int Id { get; set; }
        public List<DamBordVakje> DamBordVakjes { get; set; }

        public DamBord()
        {
            DamBordVakjes = VulBord();
        }

        private List<DamBordVakje> VulBord()
        {
            List<DamBordVakje> Vakjes = new List<DamBordVakje>(); // initialize the list
                                                                                // add tuples representing the squares on the board
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    if ((row + col) % 2 == 0)
                    {
                        if (row < 3) Vakjes.Add(new DamBordVakje(row, col, new DamStuk(Kleur.Wit, Type.Schijf))); // white piece
                        else if (row > 4) Vakjes.Add(new DamBordVakje(row, col, new DamStuk(Kleur.Zwart, Type.Schijf))); // black piece
                    }
                }
            }
            return Vakjes;
        }
    }
}
