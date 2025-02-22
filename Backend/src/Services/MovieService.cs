﻿using Microsoft.EntityFrameworkCore;
using VideoRentalService.DBContext;
using VideoRentalService.Models;

namespace VideoRentalService.Services
{
    public class MovieService
    {

        private readonly VideoRentalServiceContext _context;

        public MovieService(VideoRentalServiceContext context)
        {
            _context = context;
        }

        // CRUD for Movie
        public async Task<List<Movie>> GetAllMoviesAsync() => await _context.Movies.ToListAsync();
        public async Task<Movie> GetMovieByIdAsync(int id) => await _context.Movies.FindAsync(id);
        public async Task<List<Movie>> FindMoviesByNameAsync(String name)
        {
            return await _context.Movies
                .Where(m => EF.Functions.Like(m.Title, $"%{name}%")) // Use LIKE for partial matching
                .ToListAsync();
        }

        public async Task<Movie> AddMovieAsync(Movie movie)
        {
            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();
            return movie;
        }

        public async Task<Movie> UpdateMovieAsync(int id, Movie updatedMovie)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null) return null;

            movie.Title = updatedMovie.Title;
            movie.Genres = updatedMovie.Genres;
            movie.CreationTime = updatedMovie.CreationTime;
            movie.UpdateTime = updatedMovie.UpdateTime;

            _context.Movies.Update(movie);
            await _context.SaveChangesAsync();
            return movie;
        }

        public async Task<bool> DeleteMovieAsync(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null) return false;

            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Movie>> GetMoviesByGenreAsync(string genre)
        {
            return await _context.Movies
                .Where(m => EF.Functions.Like(m.Genres, $"%{genre}%")) // Use LIKE for partial matching
                .ToListAsync();
        }
    }
}
