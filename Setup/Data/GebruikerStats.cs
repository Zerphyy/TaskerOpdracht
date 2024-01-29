using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Setup.Data
{
    public class GebruikerStats
    {
        [Key]
        public int ID { get; set; }
        [Key]
        public Gebruiker Speler { get; set; }
        public int AantalSpellen { get; set; }
        public int AantalGewonnen { get; set; }
        public int AantalVerloren { get; set; }
        public int WinLossRatio { get; set; }
        public int LangsteWinstreak { get; set; }

        public GebruikerStats() { }
        public GebruikerStats(int iD, Gebruiker speler, int aantalSpellen, int aantalGewonnen, int aantalVerloren, int winLossRatio, int langsteWinstreak)
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
