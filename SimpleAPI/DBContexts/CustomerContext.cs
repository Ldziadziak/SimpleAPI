using Microsoft.EntityFrameworkCore;
using SimpleAPI.Entities;

namespace SimpleAPI.DbContexts;

public class CustomerContext : DbContext
{
    //dotnet ef migrations add InitialCreate
    public DbSet<Customer> customer { get; set; } = null!;
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Filename=DB/MyDatabase.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>()
            .HasData(
           new Customer()
           {
               Id = 1,
               Name = "Karsa",
               Surname = "Orlong"
           },
           new Customer()
           {
               Id = 2,
               Name = "Anomander",
               Surname = "Rake"
           },
           new Customer()
           {
               Id = 3,
               Name = "Onos",
               Surname = "T'oolan"
           });

        base.OnModelCreating(modelBuilder);
    }


}
