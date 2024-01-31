using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Xml;

namespace Setup.Data
{
    public class WebpageDBContext:IdentityDbContext
    {
        public DbSet<ContactData>? ContactData { get; set; }
        public DbSet<DamSpel>? DamSpel { get; set; }
        public DbSet<DamBord>? DamBord { get; set; }
        public DbSet<DamBordVakje>? DamBordVakje { get; set; }
        public DbSet<DamStuk>? DamStuk { get; set; }
        public DbSet<Gebruiker>? Speler { get; set; }
        public DbSet<GebruikerStats>? SpelerStats { get; set; }

        public WebpageDBContext(DbContextOptions<WebpageDBContext> options) : base(options)
        {
        }
        public WebpageDBContext()
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=TaskerOpdrachtDb;Trusted_Connection=True;");
            } else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                optionsBuilder.UseSqlServer("Server=localhost;Database=TaskerOpdrachtDb;User ID=SA;Password=Plusklas01;");
            }
        }
        //seeden van DB tabel (zouu verouderd kunnen zijn)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //geef base db creation values voor alle standard tables
            base.OnModelCreating(modelBuilder);
            //seed tables die niet aangepast hoeven te worden
            modelBuilder.Entity<DamStuk>().HasData(new DamStuk {Id = 1, Kleur = Kleur.Wit, Type = Type.Schijf });
            modelBuilder.Entity<DamStuk>().HasData(new DamStuk {Id = 2, Kleur = Kleur.Zwart, Type = Type.Schijf });
            modelBuilder.Entity<DamStuk>().HasData(new DamStuk {Id = 3, Kleur = Kleur.Wit, Type = Type.Dam });
            modelBuilder.Entity<DamStuk>().HasData(new DamStuk {Id = 4, Kleur = Kleur.Zwart, Type = Type.Dam });
        }

    }
}
