using Microsoft.EntityFrameworkCore;
using VideoRentalService.Models;

namespace VideoRentalService.DBContext
{
    public class VideoRentalServiceContext : DbContext
    {
        public VideoRentalServiceContext(DbContextOptions<VideoRentalServiceContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Rental> Rentals { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // One User has many Rentals
            modelBuilder.Entity<Rental>()
                .HasOne(r => r.User)
                .WithMany(u => u.Rentals)
                .HasForeignKey(r => r.UserId);

            // One Rental is associated with one Movie
            modelBuilder.Entity<Rental>()
                .HasOne(r => r.Movie)
                .WithMany(m => m.Rentals)
                .HasForeignKey(r => r.MovieId);

            modelBuilder.Entity<Rental>()
                .Property(r => r.Penalty)
                .HasPrecision(18, 2); // Precision: 18 total digits, Scale: 2 decimal places


            // Configure CreationTime and UpdateTime defaults
            modelBuilder.Entity<User>()
                .Property(u => u.CreationTime)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.Entity<User>()
                .Property(u => u.UpdateTime)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.Entity<Movie>()
                .Property(m => m.CreationTime)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.Entity<Movie>()
                .Property(m => m.UpdateTime)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.Entity<Rental>()
                .Property(r => r.CreationTime)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.Entity<Rental>()
                .Property(r => r.UpdateTime)
                .HasDefaultValueSql("GETUTCDATE()");

            // Seed initial users
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserId = 1,
                    Username = "testEmployee",
                    Password = "employee123",
                    Role = "Employee"
                },
                new User
                {
                    UserId = 2,
                    Username = "testUser",
                    Password = "user123",
                    Role = "User"
                }
            );

            // Seed initial Movies
            modelBuilder.Entity<Movie>().HasData(
                new Movie
                {
                    MovieId = 1,
                    Title = "The Shawshank Redemption",
                    Type = "DVD"
                },
                new Movie
                {
                    MovieId = 2,
                    Title = "The Godfather",
                    Type = "Blu-ray"
                },
                new Movie
                {
                    MovieId = 3,
                    Title = "Inception",
                    Type = "Digital"
                }
            );

            // Seed Rentals (One Rental = One Movie)
            modelBuilder.Entity<Rental>().HasData(
                new Rental
                {
                    RentalId = 1,
                    UserId = 2, // Refers to testUser
                    MovieId = 1, // Refers to The Shawshank Redemption
                    RentalDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    ReturnDate = null,
                    IsDamaged = false,
                    Penalty = 0
                },
                new Rental
                {
                    RentalId = 2,
                    UserId = 2, // Refers to testUser
                    MovieId = 2, // Refers to The Godfather
                    RentalDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    ReturnDate = null,
                    IsDamaged = false,
                    Penalty = 0
                }
            );
        }

        // Override ensures that UpdateTime is automatically updated whenever an entity is modified and CreationTime is always added
        public override int SaveChanges()
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is User || e.Entity is Movie || e.Entity is Rental);

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    ((dynamic)entry.Entity).CreationTime = DateTime.UtcNow;
                    ((dynamic)entry.Entity).UpdateTime = DateTime.UtcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    ((dynamic)entry.Entity).UpdateTime = DateTime.UtcNow;
                }
            }

            return base.SaveChanges();
        }

    }
}
