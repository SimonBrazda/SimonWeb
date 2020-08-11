using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CL3C.Models
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BaseCar>().HasData(
                new BaseCar
                {
                    ID = 1,
                    Name = "Car A",
                    Owner = "nomis",
                    PurchasePrice = 200000,
                    TechnicalLife = 200000,
                    FuelPrice = 30.0M,
                    FuelConsumption = 7.5M
                },
                new BaseCar
                {
                    ID = 2,
                    Name = "Car B",
                    Owner = "nomis",
                    PurchasePrice = 150000,
                    TechnicalLife = 150000,
                    FuelPrice = 30.0M,
                    FuelConsumption = 5.5M
                }
            );

            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Name = "SuperAdmin",
                    NormalizedName = "SUPERADMIN",
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                },
                new IdentityRole
                {
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                },
                new IdentityRole
                {
                    Name = "User",
                    NormalizedName = "USER",
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                }
            );
        }
    }
}