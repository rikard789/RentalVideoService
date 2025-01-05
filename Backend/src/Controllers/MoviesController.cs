using Microsoft.AspNetCore.Mvc;
using VideoRentalService.Models;
using VideoRentalService.Services;

namespace VideoRentalService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly MovieService _movieService;

        public MoviesController(MovieService movieService)
        {
            _movieService = movieService;
        }

        // Get all movies which contain name
        [HttpGet("search")]
        public async Task<ActionResult<List<Movie>>> FindMoviesByName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("Name parameter cannot be null or empty.");
            }

            var movies = await _movieService.FindMoviesByNameAsync(name);

            if (movies == null || movies.Count == 0)
            {
                return NotFound($"No movies found with name containing '{name}'.");
            }

            return Ok(movies);
        }

        // Get movie by id
        [HttpGet("search/{id}")]
        public async Task<ActionResult<List<Movie>>> FindMovieById(int id)
        {
            if (id < 1)
            {
                return BadRequest("Name parameter cannot be lesser than 1.");
            }

            var movie = await _movieService.GetMovieByIdAsync(id);

            return Ok(movie);
        }

        // Get all movies
        [HttpGet("getAll")]
        public async Task<ActionResult<List<Movie>>> GetAllMovies()
        {
            var movies = await _movieService.GetAllMoviesAsync();
            return Ok(movies);
        }

        // Create a new movie
        [HttpPost]
        public async Task<ActionResult<Movie>> CreateMovie(Movie movie)
        {
            var newMovie = await _movieService.AddMovieAsync(movie);
            return CreatedAtAction(nameof(FindMovieById), new { id = newMovie.MovieId }, newMovie);
        }

        // Update an existing movie
        [HttpPut("{id}")]
        public async Task<ActionResult<Movie>> UpdateMovie(int id, Movie updatedMovie)
        {
            var movie = await _movieService.UpdateMovieAsync(id, updatedMovie);
            if (movie == null) return NotFound($"Movie with ID {id} not found.");

            return Ok(movie);
        }

        // Delete a movie
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMovie(int id)
        {
            var success = await _movieService.DeleteMovieAsync(id);
            if (!success) return NotFound($"Movie with ID {id} not found.");

            return NoContent();
        }
    }
}
