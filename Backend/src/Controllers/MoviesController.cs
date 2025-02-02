using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VideoRentalService.Models;
using VideoRentalService.Services;

namespace VideoRentalService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MoviesController : ControllerBase
    {
        private readonly MovieService _movieService;

        public MoviesController(MovieService movieService)
        {
            _movieService = movieService;
        }

        // Get all movies which contain name
        [HttpGet("searchByName/{name}")]
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
        [HttpGet("searchById/{id}")]
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
        [HttpGet("getAllMovies")]
        public async Task<ActionResult<List<Movie>>> GetAllMovies()
        {
            var movies = await _movieService.GetAllMoviesAsync();
            return Ok(movies);
        }

        // Create a new movie
        [HttpPost("createMovie")]
        public async Task<ActionResult<Movie>> CreateMovie(Movie movie)
        {
            var newMovie = await _movieService.AddMovieAsync(movie);
            return CreatedAtAction(nameof(FindMovieById), new { id = newMovie.MovieId }, newMovie);
        }

        // Update an existing movie
        [HttpPut("updateMovie/{id}")]
        public async Task<ActionResult<Movie>> UpdateMovie(int id, Movie updatedMovie)
        {
            var movie = await _movieService.UpdateMovieAsync(id, updatedMovie);
            if (movie == null) return NotFound($"Movie with ID {id} not found.");

            return Ok(movie);
        }

        // Delete a movie
        [HttpDelete("deleteMovie/{id}")]
        public async Task<ActionResult> DeleteMovie(int id)
        {
            var success = await _movieService.DeleteMovieAsync(id);
            if (!success) return NotFound($"Movie with ID {id} not found.");

            return NoContent();
        }

        [HttpGet("genre/{genre}")]
        public async Task<ActionResult<List<Movie>>> GetMoviesByGenre(string genre)
        {
            if (string.IsNullOrEmpty(genre))
            {
                return BadRequest("Genre parameter cannot be null or empty.");
            }

            var movies = await _movieService.GetMoviesByGenreAsync(genre);

            if (movies == null || movies.Count == 0)
            {
                return NotFound($"No movies found for genre '{genre}'.");
            }

            return Ok(movies);
        }
    }
}
