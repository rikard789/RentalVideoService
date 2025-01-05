using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideoRentalService.Controllers;
using VideoRentalService.DBContext;
using VideoRentalService.Models;
using VideoRentalService.Services;
using Xunit;

namespace test.Controllers
{
    public class RentalControllerTests
    {
        private RentalController _controller;
        private RentalService _rentalService;
        private VideoRentalServiceContext _context;

        public RentalControllerTests()
        {
            var options = new DbContextOptionsBuilder<VideoRentalServiceContext>()
                .UseInMemoryDatabase(databaseName: "TestRentalDatabase")
                .Options;

            _context = new VideoRentalServiceContext(options);

            removeRentals();

            _rentalService = new RentalService(_context);
            _controller = new RentalController(_rentalService);
        }

        [Fact]
        public async Task GetAllRentals_Success()
        {
            removeRentals();

            var movie = new Movie { MovieId = 1, Title = "Test", Genres = "Action", Type = "DVD" };
            var user = new User { UserId = 1, Username = "Test", Password = "Test", Role = "User" };
            var rental = new Rental { RentalId = 1, UserId = user.UserId, MovieId = movie.MovieId, RentalDate = DateTime.Now, IsDamaged = false, Movie = movie, User = user };
            _context.Rentals.Add(rental);
            _context.SaveChanges();

            var result = await _controller.GetAllRentalsForUser(1);

            var actionResult = Assert.IsType<ActionResult<List<Rental>>>(result);
            var rentals = Assert.IsType<List<Rental>>(actionResult.Value);
            Assert.Single(rentals);
        }

        [Fact]
        public async Task GetAllRentals_Empty()
        {
            removeRentals();

            var result = await _controller.GetAllRentalsForUser(2);

            var actionResult = Assert.IsType<ActionResult<List<Rental>>>(result);
            var rentals = Assert.IsType<List<Rental>>(actionResult.Value);
            Assert.Empty(rentals);
        }

        [Fact]
        public async Task AddMovieToRental_Success()
        {
            var result = await _controller.AddMovieToRental(1, 1);

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task SetRentalPenalty_Success()
        {
            var result = await _controller.SetRentalPenalty(1, true, Decimal.One);

            Assert.IsType<OkResult>(result);
        }

        private void removeRentals()
        {
            _context.Rentals.RemoveRange(_context.Rentals);
            _context.SaveChanges();
        }
    }
}