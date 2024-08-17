using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;
using Store_API.Models;

namespace Store_API.Database
{
    public class StoresDbContext(DbContextOptions<StoresDbContext> options) : DbContext(options)
    {
        public DbSet<Store> Stores { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Store>().ToCollection("stores");
        }
    }
}
