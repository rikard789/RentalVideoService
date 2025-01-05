using Microsoft.AspNetCore.Mvc;
using VideoRentalService.Models;
using VideoRentalService.Services;

namespace VideoRentalService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RentalController : ControllerBase
    {
        private readonly RentalService _rentalService;

        public RentalController(RentalService rentalService)
        {
            _rentalService = rentalService;
        }

        // Get all rentals for user
        [HttpGet("allRentals")]
        public async Task<ActionResult<List<Rental>>> GetAllRentalsForUser(int userId)
        {
            return await _rentalService.GetRentalsForUserAsync(userId);
        }

        //Add movie to rental
        [HttpPost("addMovie")]
        public async Task<ActionResult> AddMovieToRental(int userId, int movieId)
        {
            await _rentalService.AddRentalMovieAsync(userId, movieId);
            return Ok();
        }

        // Set rental penalty for damaged movie
        [HttpPost("set-penalty")]
        public async Task<ActionResult> SetRentalPenalty(int rentalId, bool isDamaged, decimal penalty)
        {
            await _rentalService.SetPenaltyForDamagedMovieAsync(rentalId, isDamaged, penalty);
            return Ok();
        }
    }
}
