using Microsoft.EntityFrameworkCore;
using VideoRentalService.DBContext;
using VideoRentalService.Models;

namespace VideoRentalService.Services
{
    public class UserService
    {
        private readonly VideoRentalServiceContext _context;

        public UserService(VideoRentalServiceContext context)
        {
            _context = context;
        }

        // CRUD for User
        public async Task<List<User>> GetAllUsersAsync() => await _context.Users.ToListAsync();
        public async Task<User> GetUserByIdAsync(int id) => await _context.Users.FindAsync(id);

        public async Task<List<User>> FindUsersByUsernameAsync(String username)
        {
            return await _context.Users
                .Where(u => EF.Functions.Like(u.Username, $"%{username}%")) // Use LIKE for partial matching
                .ToListAsync();
        }

        public async Task<User> AddUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateUserAsync(int id, User updatedUser)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return null;

            user.Username = updatedUser.Username;
            user.Role = updatedUser.Role;

            _context.Users.Update(user);

            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<User> AuthenticateUserAsync(string username, string password)
        {
            return await _context.Users.FirstOrDefaultAsync(u =>
                u.Username == username && u.Password == password);
        }

    }
}
