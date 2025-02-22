﻿using Microsoft.EntityFrameworkCore;
using VideoRentalService.DBContext;
using VideoRentalService.Models;

namespace VideoRentalService.Services
{
    public class RentalService
    {
        private readonly VideoRentalServiceContext _context;

        public RentalService(VideoRentalServiceContext context)
        {
            _context = context;
        }

        public async Task<List<Rental>> GetRentalsForUserAsync(int userId)
        {
            return await _context.Rentals
                .Where(r => r.UserId == userId)
                .Select(r => new Rental
                {
                    RentalId = r.RentalId,
                    UserId = r.UserId,
                    MovieId = r.MovieId,
                    RentalDate = r.RentalDate,
                    ReturnDate = r.ReturnDate,
                    IsDamaged = r.IsDamaged,
                    Penalty = r.Penalty,
                    IsPaid = r.IsPaid,
                    CreationTime = r.CreationTime,
                    UpdateTime = r.UpdateTime,
                    Movie = new Movie
                    {
                        MovieId = r.Movie.MovieId,
                        Title = r.Movie.Title,
                        Genres = r.Movie.Genres,
                        Type = r.Movie.Type,
                    }
                })
                .ToListAsync();
        }


        public async Task AddRentalMovieAsync(int userId, int movieId) {
            var rental = new Rental{
                UserId = userId,
                MovieId = movieId,
                RentalDate = DateTime.Now
            };
            _context.Rentals.Add(rental);
            await _context.SaveChangesAsync();
        }

        public async Task SetPenaltyForDamagedMovieAsync(int rentalId, bool isDamaged, decimal penalty) { 
            var rental = await _context.Rentals.FindAsync(rentalId);
            if (rental != null) { 
                rental.ReturnDate = DateTime.Now;
                rental.IsDamaged = isDamaged;
                rental.Penalty = penalty;
                await _context.SaveChangesAsync();
            }
        }
    }
}
