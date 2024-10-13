using System.ComponentModel.DataAnnotations;

namespace Setup.Data
{
    public class GebruikerStats
    {
        [Key]
        public string Speler { get; set; }
        public int AantalSpellen { get; set; }
        public int AantalGewonnen { get; set; }
        public int AantalVerloren { get; set; }
        public int WinLossRatio { get; set; }

        public GebruikerStats() { }
        public GebruikerStats(int iD, string speler, int aantalSpellen, int aantalGewonnen, int aantalVerloren, int winLossRatio, int langsteWinstreak)
        {
            Speler = speler;
            AantalSpellen = aantalSpellen;
            AantalGewonnen = aantalGewonnen;
            AantalVerloren = aantalVerloren;
            WinLossRatio = winLossRatio;
        }
    }
}
