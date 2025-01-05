using Microsoft.EntityFrameworkCore;
using VideoRentalService.DBContext;
using VideoRentalService.Models;
using VideoRentalService.Services;
using Xunit;

namespace test.Services
{
    public class RentalServiceTests
    {
        private readonly RentalService _service;
        private readonly VideoRentalServiceContext _context;

        public RentalServiceTests()
        {
            var options = new DbContextOptionsBuilder<VideoRentalServiceContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new VideoRentalServiceContext(options);
            _service = new RentalService(_context);

            SeedData();
        }

        private void SeedData()
        {
            _context.Users.Add(new User { UserId = 1, Username = "User1", Password = "Pass1", Role = "User" });
            _context.Movies.Add(new Movie { MovieId = 1, Title = "Movie1", Genres = "Action", Type = "DVD" });
            _context.Movies.Add(new Movie { MovieId = 2, Title = "Movie2", Genres = "Comedy", Type = "Electronic" });
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetRentalsForUserAsync_ReturnsUserRentals()
        {
            var rental1 = new Rental { RentalId = 1, UserId = 1, MovieId = 1, RentalDate = DateTime.Now };
            var rental2 = new Rental { RentalId = 2, UserId = 1, MovieId = 2, RentalDate = DateTime.Now };
            _context.Rentals.Add(rental1);
            _context.Rentals.Add(rental2);
            _context.SaveChanges();

            var result = await _service.GetRentalsForUserAsync(1);

            Assert.Equal(2, result.Count);
            Assert.Contains(result, r => r.MovieId == 1);
            Assert.Contains(result, r => r.MovieId == 2);
        }

        [Fact]
        public async Task AddRentalMovieAsync_AddsRental()
        {
            await _service.AddRentalMovieAsync(1, 1);

            var rental = await _context.Rentals.FirstOrDefaultAsync(r => r.UserId == 1 && r.MovieId == 1);
            Assert.NotNull(rental);
            Assert.Equal(1, rental.UserId);
            Assert.Equal(1, rental.MovieId);
        }

        [Fact]
        public async Task SetPenaltyForDamagedMovieAsync_UpdatesRental()
        {
            var rental = new Rental { RentalId = 1, UserId = 1, MovieId = 1, RentalDate = DateTime.Now };
            _context.Rentals.Add(rental);
            _context.SaveChanges();

            await _service.SetPenaltyForDamagedMovieAsync(1, true, 50m);

            Assert.True(rental.IsDamaged);
            Assert.Equal(50m, rental.Penalty);
            Assert.NotNull(rental.ReturnDate);
        }

        [Fact]
        public async Task SetPenaltyForDamagedMovieAsync_NoRentalFound()
        {
            await _service.SetPenaltyForDamagedMovieAsync(999, true, 100m);

            var rental = await _context.Rentals.FindAsync(999);
            Assert.Null(rental);
        }
    }
}