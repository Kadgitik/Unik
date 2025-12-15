using Microsoft.EntityFrameworkCore;
using TravelTracker.Models;

namespace TravelTracker.Data
{
    public class TravelDbContext : DbContext
    {
        public DbSet<Trip> Trips { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<TripType> TripTypes { get; set; }
        public DbSet<Accommodation> Accommodations { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=travel.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Налаштування зв'язків
            modelBuilder.Entity<Trip>()
                .HasOne(t => t.Country)
                .WithMany(c => c.Trips)
                .HasForeignKey(t => t.CountryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Trip>()
                .HasOne(t => t.TripType)
                .WithMany(tt => tt.Trips)
                .HasForeignKey(t => t.TripTypeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Trip>()
                .HasOne(t => t.Accommodation)
                .WithMany(a => a.Trips)
                .HasForeignKey(t => t.AccommodationId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed data
            modelBuilder.Entity<Country>().HasData(
                new Country { CountryId = 1, Name = "Україна", Code = "UA" },
                new Country { CountryId = 2, Name = "Польща", Code = "PL" },
                new Country { CountryId = 3, Name = "Італія", Code = "IT" },
                new Country { CountryId = 4, Name = "Франція", Code = "FR" },
                new Country { CountryId = 5, Name = "Німеччина", Code = "DE" }
            );

            modelBuilder.Entity<TripType>().HasData(
                new TripType { TripTypeId = 1, Name = "Відпочинок" },
                new TripType { TripTypeId = 2, Name = "Робота" },
                new TripType { TripTypeId = 3, Name = "Екскурсія" },
                new TripType { TripTypeId = 4, Name = "Відрядження" }
            );

            modelBuilder.Entity<Accommodation>().HasData(
                new Accommodation { AccommodationId = 1, Name = "Готель" },
                new Accommodation { AccommodationId = 2, Name = "Хостел" },
                new Accommodation { AccommodationId = 3, Name = "Airbnb" },
                new Accommodation { AccommodationId = 4, Name = "У друзів" }
            );
        }
    }
}