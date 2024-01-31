using System.ComponentModel.DataAnnotations;

namespace Setup.Data
{
    public class DamBord
    {
        public int Id { get; set; }


        public DamBord() { }
        public DamBord(int id)
        {
            this.Id = id;
            VulBord(id);
        }

        private void VulBord(int id)
        {
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    if ((row + col) % 2 == 0)
                    {
                        if (row < 3) new DamBordVakje(Id = id, row, col); // white piece
                        else if (row > 4) new DamBordVakje(Id = id, row, col); // black piece
                    }
                }
            }
        }
    }
}
