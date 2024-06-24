using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Setup.Data
{
    public class SpelerStats
    {
        [Key]
        public int ID { get; set; }
        [Key]
        public Speler Speler { get; set; }
        public int AantalSpellen { get; set; }
        public int AantalGewonnen { get; set; }
        public int AantalVerloren { get; set; }
        public int WinLossRatio { get; set; }
        public int LangsteWinstreak { get; set; }

        public SpelerStats() { }
        public SpelerStats(int iD, Speler speler, int aantalSpellen, int aantalGewonnen, int aantalVerloren, int winLossRatio, int langsteWinstreak)
        {
            ID = iD;
            Speler = speler;
            AantalSpellen = aantalSpellen;
            AantalGewonnen = aantalGewonnen;
            AantalVerloren = aantalVerloren;
            WinLossRatio = winLossRatio;
            LangsteWinstreak = langsteWinstreak;
        }
    }
}
