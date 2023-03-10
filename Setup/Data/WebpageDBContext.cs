using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Setup.Data
{
    public class WebpageDBContext:DbContext
    {
        public DbSet<ContactData> ContactData { get; set; }
        public DbSet<DamSpel> DamSpel { get; set; }
        public DbSet<DamBord> DamBord { get; set; }
        public DbSet<DamBordVakje> DamBordVakje { get; set; }
        public DbSet<DamStuk> DamStuk { get; set; }
        public DbSet<Speler> Speler { get; set; }
        public DbSet<SpelerStats> SpelerStats { get; set; }

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
    }
}
