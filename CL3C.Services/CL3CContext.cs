using System;
using CL3C.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Linq;

namespace CL3C.Services
{
    public class CL3CContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<BaseCar> Cars { get; set; }

        public CL3CContext(DbContextOptions<CL3CContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Seed();

            foreach (var foreignKey in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }

            // base.OnModelCreating(modelBuilder);

            // modelBuilder.Entity<BaseCar>()
            //     .Property(c => c.ID)
            //     .IsRequired()
            //     .HasMaxLength(10);
            
            // modelBuilder.Entity<BaseCar>()
            //     .Property(c => c.BaseLifeCycleCosts)
            //     .ValueGeneratedOnUpdate();
            
            // modelBuilder.Entity<BaseCar>()
            //     .Property(c => c.BaseCostsPerDistanceUnit)
            //     .ValueGeneratedOnUpdate();
        }
    }
}
