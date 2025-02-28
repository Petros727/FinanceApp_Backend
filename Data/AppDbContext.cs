using FinancialApp.Models;
using Microsoft.EntityFrameworkCore;

namespace FinancialApp.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Crypto> Cryptos { get; set; }
        public DbSet<User> Users { get; set; } 

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Stock>()
                .HasKey(s => new { s.Symbol, s.Date });

            modelBuilder.Entity<Crypto>()
                .HasKey(c => new { c.Symbol, c.Date });

            base.OnModelCreating(modelBuilder);
        }

       
    }
}