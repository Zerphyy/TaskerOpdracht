using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Setup.Controllers;
using System.Runtime.InteropServices;

namespace Setup.Data
{
    public class WebpageDBContext:IdentityDbContext
    {
        public DbSet<ContactData>? ContactData { get; set; }
        public DbSet<DamSpel>? DamSpel { get; set; }
        public DbSet<DamBord>? DamBord { get; set; }
        public DbSet<Gebruiker>? Speler { get; set; }
        public DbSet<GebruikerStats>? SpelerStats { get; set; }

        public WebpageDBContext(DbContextOptions<WebpageDBContext> options) : base(options)
        {
            Console.WriteLine(options);
        }
        //seeden van DB tabel (zouu verouderd kunnen zijn)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //geef base db creation values voor alle standard tables
            base.OnModelCreating(modelBuilder);
            //seed tables die niet aangepast hoeven te worden
            modelBuilder.Entity<Gebruiker>().HasData(new Gebruiker {Naam = "Zerphy", Email = "kevinspijker@kpnmail.nl", Wachtwoord = PasswordManager.HashPassword("Plusklas01!"), Rol = "Moderator" });
        }

    }
}
