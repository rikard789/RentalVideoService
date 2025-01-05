using Microsoft.EntityFrameworkCore;
using VideoRentalService.DBContext;
using VideoRentalService.Models;
using VideoRentalService.Services;
using Xunit;

namespace test.Services
{
    public class MovieServiceTests
    {
        private readonly MovieService _service;
        private readonly VideoRentalServiceContext _context;

        public MovieServiceTests()
        {
            var options = new DbContextOptionsBuilder<VideoRentalServiceContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new VideoRentalServiceContext(options);
            _service = new MovieService(_context);
        }

        [Fact]
        public async Task GetAllMoviesAsync_ReturnsListOfMovies()
        {
            _context.Movies.Add(new Movie { MovieId = 1, Title = "Movie 1", Genres = "Action", Type = "DVD" });
            _context.Movies.Add(new Movie { MovieId = 2, Title = "Movie 2", Genres = "Horror", Type = "Electronic" });
            _context.SaveChanges();

            var result = await _service.GetAllMoviesAsync();

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetMovieByIdAsync_ReturnsMovie()
        {
            var movie = new Movie { MovieId = 1, Title = "Movie 1", Genres = "Action", Type = "DVD" };
            _context.Movies.Add(movie);
            _context.SaveChanges();

            var result = await _service.GetMovieByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal(movie.MovieId, result.MovieId);
            Assert.Equal(movie.Title, result.Title);
        }

        [Fact]
        public async Task FindMoviesByNameAsync_ReturnsMatchingMovies()
        {
            _context.Movies.Add(new Movie { MovieId = 1, Title = "Action Movie", Genres = "Action", Type = "DVD" });
            _context.Movies.Add(new Movie { MovieId = 2, Title = "Horror Flick", Genres = "Horror", Type = "Electronic" });
            _context.SaveChanges();

            var result = await _service.FindMoviesByNameAsync("Action");

            Assert.Single(result);
            Assert.Equal("Action Movie", result[0].Title);
        }

        [Fact]
        public async Task AddMovieAsync_AddsMovie()
        {
            var movie = new Movie { MovieId = 0, Title = "New Movie", Genres = "Comedy", Type = "DVD" };

            var addedMovie = await _service.AddMovieAsync(movie);

            Assert.NotNull(addedMovie);
            Assert.Equal("New Movie", addedMovie.Title);

            var dbMovie = await _context.Movies.FindAsync(addedMovie.MovieId);
            Assert.NotNull(dbMovie);
            Assert.Equal("New Movie", dbMovie.Title);
        }

        [Fact]
        public async Task UpdateMovieAsync_UpdatesMovie()
        {
            var movie = new Movie { MovieId = 1, Title = "Original Movie", Genres = "Drama", Type = "DVD" };
            _context.Movies.Add(movie);
            _context.SaveChanges();

            var updatedMovie = new Movie { Title = "Updated Movie", Genres = "Action", Type = "Electronic" };

            var result = await _service.UpdateMovieAsync(1, updatedMovie);

            Assert.NotNull(result);
            Assert.Equal("Updated Movie", result.Title);

            var movieInDb = await _context.Movies.FindAsync(1);
            Assert.NotNull(movieInDb);
            Assert.Equal("Updated Movie", movieInDb.Title);
        }

        [Fact]
        public async Task DeleteMovieAsync_RemovesMovie()
        {
            var movie = new Movie { MovieId = 1, Title = "Delete Movie", Genres = "Horror", Type = "DVD" };
            _context.Movies.Add(movie);
            _context.SaveChanges();

            var result = await _service.DeleteMovieAsync(1);

            Assert.True(result);

            var deletedMovie = await _context.Movies.FindAsync(1);
            Assert.Null(deletedMovie);
        }
    }
}