using System.ComponentModel.DataAnnotations;

namespace Setup.Data
{
    public class DamStuk
    {
        public int Id { get; set; }
        [Required]
        public Kleur Kleur { get; set; }
        [Required]
        public Type Type { get; set; }

        public DamStuk() { }
        public DamStuk(Kleur kleur, Type type) { this.Kleur = kleur; this.Type = type; }
    }
    public enum Kleur
    {
        Wit,
        Zwart
    }
    public enum Type
    {
        Schijf,
        Dam
    }
}
