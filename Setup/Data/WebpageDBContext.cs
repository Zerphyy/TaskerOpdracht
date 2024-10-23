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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Gebruiker>().HasData(new Gebruiker {Naam = "Zerphy", Email = "kevinspijker@kpnmail.nl", Wachtwoord = PasswordManager.HashPassword("Plusklas01!"), Rol = "Admin" });
            modelBuilder.Entity<Gebruiker>().HasData(new Gebruiker { Naam = "TestUser", Email = "testmail@example.com", Wachtwoord = PasswordManager.HashPassword("testUser123!"), Rol = "Gebruiker" });
        }

    }
}