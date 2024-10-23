
namespace Setup.Data
{
    public class Personalia
    {
        public string Naam { get; set; }
        public string Email { get; set; }
        public string Telefoonnummer { get; set; }
        public string Straat { get; set; }
        public string Zip { get; set; }
        public DateTime Verjaardag { get; set; }
        public string LinkedIn { get; set; }
        public int BIG { get; set; }
        public List<string> Eigenschappen { get; set; }
        public Dictionary<string, string> Vaardigheden { get; set; }
        public Profiel? Profiel { get; set; }
    }
    public class Profiel
    {
        public string? Beschrijving { get; set; }
        public List<Opleiding>? Opleidingen { get; set; }
        public List<Cursus>? Cursussen { get; set; }
        public List<Stage>? Stages { get; set; }
        public List<Werkervaring>? Werkervaringen { get; set; }
    }
    public class Opleiding
    {
        public string Naam { get; set; }
        public string Plaats { get; set; }
        public DateTime Van { get; set; }
        public DateTime Tot { get; set; }
    }
    public class Cursus
    {
        public string Naam { get; set; }
        public int JaarVanUitvoering { get; set; }
    }
    public class Stage
    {
        public string Naam { get; set; }
        public string Plaats { get; set; }
        public DateTime Van { get; set; }
        public DateTime Tot { get; set; }
    }
    public class Werkervaring
    {
        public string Naam { get; set; }
        public string Plaats { get; set; }
        public DateTime Van { get; set; }
        public DateTime Tot { get; set; }
    }
}
