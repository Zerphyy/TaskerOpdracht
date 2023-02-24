using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Setup.Data
{
    public class ContactDataDBContext:DbContext
    {
        public DbSet<ContactData> ContactData { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=TaskerOpdrachtDb;Trusted_Connection=True;");
        }
    }
}
