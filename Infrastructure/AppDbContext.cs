using Domain;
using Domain.Entities;
using Infrastructure.Configuration;
using Microsoft.EntityFrameworkCore;


namespace WebApplication1.Data
{
    public class AppDbContext : DbContext
    {

        public DbSet<Client> Clients { get; set; }
        public DbSet<Edition> Editions { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Exemplar> Exemplars { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<Return> Returns { get; set; }
        public DbSet<SaleAndExemplar> SalesAndExemplars { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
       : base(options)  
        {

        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ClientConfiguration());
            modelBuilder.ApplyConfiguration(new EditionConfiguration());
            modelBuilder.ApplyConfiguration(new EmployeeConfiguration());
            modelBuilder.ApplyConfiguration(new ExemplarConfiguration());
            modelBuilder.ApplyConfiguration(new SaleConfiguration());
            modelBuilder.ApplyConfiguration(new ReturnConfiguration());
            modelBuilder.ApplyConfiguration(new SaleAndExemplarConfiguration());

            base.OnModelCreating(modelBuilder);
        }

    }
}
