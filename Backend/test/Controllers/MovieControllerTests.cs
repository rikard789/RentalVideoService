using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideoRentalService.Controllers;
using VideoRentalService.DBContext;
using VideoRentalService.Models;
using VideoRentalService.Services;
using Xunit;

namespace test.Controllers
{
    public class MoviesControllerTests
    {
        private MoviesController _controller;
        private MovieService _movieService;
        private VideoRentalServiceContext _context;

        public MoviesControllerTests()
        {
            var options = new DbContextOptionsBuilder<VideoRentalServiceContext>()
                .UseInMemoryDatabase(databaseName: "TestMovieDatabase")
                .Options;

            _context = new VideoRentalServiceContext(options);

            removeMovies();

            _movieService = new MovieService(_context);
            _controller = new MoviesController(_movieService);
        }

        [Fact]
        public async Task FindMovieByName_Success()
        {
            removeMovies();

            var movie = new Movie { Title = "Test", Genres = "Action", Type = "DVD" };
            _context.Movies.Add(movie);
            _context.SaveChanges();

            var result = await _controller.FindMoviesByName("Test");

            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task FindMovieByName_BadRequest()
        {
            var result = await _controller.FindMoviesByName("");

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task FindMovieByName_NotFound()
        {
            var result = await _controller.FindMoviesByName("Not Exists");

            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task FindMovieById_Success()
        {
            removeMovies();

            var movie = new Movie { MovieId = 1, Title = "Test Movie", Genres = "Action", Type = "DVD" };
            _context.Movies.Add(movie);
            _context.SaveChanges();

            var result = await _controller.FindMovieById(1);

            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task FindMovieById_BadRequest()
        {
            var result = await _controller.FindMovieById(0);

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetAllMovies_ReturnsAllMovies()
        {
            removeMovies();

            var movie = new Movie { Title = "Test Movie", Genres = "Action", Type = "DVD" };
            _context.Movies.Add(movie);
            _context.SaveChanges();

            var result = await _controller.GetAllMovies();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var movies = Assert.IsType<List<Movie>>(okResult.Value);
            Assert.Single(movies);
        }

        [Fact]
        public async Task CreateMovie_Success()
        {
            removeMovies();

            var movie = new Movie { Title = "New Movie", Genres = "Comedy", Type = "Electronic" };

            var result = await _controller.CreateMovie(movie);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedMovie = Assert.IsType<Movie>(createdResult.Value);
            Assert.Equal(movie.Title, returnedMovie.Title);
        }

        [Fact]
        public async Task UpdateMovie_Success()
        {
            removeMovies();

            var movie = new Movie { MovieId = 1, Title = "Existing Movie", Genres = "Drama", Type = "DVD" };
            _context.Movies.Add(movie);
            _context.SaveChanges();

            var updatedMovie = new Movie { Title = "Updated Movie", Genres = "Action", Type = "DVD" };

            var result = await _controller.UpdateMovie(1, updatedMovie);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedMovie = Assert.IsType<Movie>(okResult.Value);
            Assert.Equal(updatedMovie.Title, returnedMovie.Title);
        }

        [Fact]
        public async Task UpdateMovie_NotFound()
        {
            removeMovies();

            var updatedMovie = new Movie { Title = "Not Exists", Genres = "Action", Type = "DVD" };

            var result = await _controller.UpdateMovie(99, updatedMovie);

            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task DeleteMovie_Success()
        {
            removeMovies();

            var movie = new Movie { MovieId = 1, Title = "Test Movie", Genres = "Action", Type = "DVD" };
            _context.Movies.Add(movie);
            _context.SaveChanges();

            var result = await _controller.DeleteMovie(1);

            Assert.IsType<NoContentResult>(result);
            Assert.Null(await _context.Movies.FindAsync(1));
        }

        [Fact]
        public async Task DeleteMovie_NotFound()
        {
            removeMovies();

            var result = await _controller.DeleteMovie(99);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        private void removeMovies()
        {
            _context.Movies.RemoveRange(_context.Movies);
            _context.SaveChanges();
        }
    }
}