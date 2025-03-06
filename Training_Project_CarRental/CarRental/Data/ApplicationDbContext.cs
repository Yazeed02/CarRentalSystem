using CarRental.Models;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Rental> Rentals { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Admin> Admins { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed Admin data
            modelBuilder.Entity<Admin>().HasData(
                new Admin
                {
                    Id = 1,
                    first_name = "Yazeed",
                    last_name = "Mwafi",
                    email = "yazeed.mwafi2002@gmail.com",
                    password = "12345678"
                });

            // Seed User data
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    first_name = "Laith",
                    last_name = "Jaber",
                    gender = "Male",
                    address = "Amman",
                    phone_no = 0777777777,
                    email = "test@gmail.com",
                    password = "12345678"
                },
                new User
                {
                    Id = 2,
                    first_name = "Leen",
                    last_name = "Srour",
                    gender = "Female",
                    address = "Amman",
                    phone_no = 0799999999,
                    email = "test2@gmail.com",
                    password = "12345678"
                });

            // Seed Vehicle data
            modelBuilder.Entity<Vehicle>().HasData(
                new Vehicle
                {
                    Id = 1,
                    brand = "Mitsubishi",
                    model = "Lancer",
                    year = 2017,
                    type = "Sedan",
                    capacity = "5",
                    price_per_day = 30,
                    location = "Amman",
                    color = "Silver",
                    imageUrl = ""
                },
                new Vehicle
                {
                    Id = 2,
                    brand = "Hyundai",
                    model = "Kona",
                    year = 2020,
                    type = "SUV",
                    capacity = "5",
                    price_per_day = 40,
                    location = "Amman",
                    color = "White",
                    imageUrl = ""
                });

            // Seed Payment data
            modelBuilder.Entity<Payment>().HasData(
                new Payment
                {
                    Id = 1,
                    CardholderName = "Yazeed",
                    CardNumber = "200052214",
                    ExpiryDate = new DateTime(2025, 12, 1),
                    CVV = 222,
                    user_Id = 1
                });

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.User)
                .WithMany(u => u.Payments)
                .HasForeignKey(p => p.user_Id)
                .OnDelete(DeleteBehavior.Restrict); // No cascading delete

            modelBuilder.Entity<Rental>()
                .HasOne(r => r.User)
                .WithMany(u => u.Rentals)
                .HasForeignKey(r => r.user_id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Rental>()
                .HasOne(r => r.Vehicle)
                .WithMany(v => v.Rentals)
                .HasForeignKey(r => r.vehicle_id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Rental>()
                .HasOne(r => r.Payment)
                .WithMany(p => p.Rentals)
                .HasForeignKey(r => r.payment_id)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
