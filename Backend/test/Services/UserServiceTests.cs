using Microsoft.EntityFrameworkCore;
using VideoRentalService.DBContext;
using VideoRentalService.Models;
using VideoRentalService.Services;
using Xunit;

namespace test.Services
{
    public class UserServiceTests
    {
        private readonly UserService _service;
        private readonly VideoRentalServiceContext _context;

        public UserServiceTests()
        {
            var options = new DbContextOptionsBuilder<VideoRentalServiceContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new VideoRentalServiceContext(options);
            _service = new UserService(_context);
        }

        [Fact]
        public async Task GetAllUsersAsync_ReturnsListOfUsers()
        {
            _context.Users.Add(new User { UserId = 1, Username = "User1", Password = "Pass1", Role = "User" });
            _context.Users.Add(new User { UserId = 2, Username = "User2", Password = "Pass2", Role = "Employee" });
            _context.SaveChanges();

            var result = await _service.GetAllUsersAsync();

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetUserByIdAsync_ReturnsUser()
        {
            var user = new User { UserId = 1, Username = "User1", Password = "Pass1", Role = "User" };
            _context.Users.Add(user);
            _context.SaveChanges();

            var result = await _service.GetUserByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal(user.UserId, result.UserId);
            Assert.Equal(user.Username, result.Username);
        }

        [Fact]
        public async Task FindUsersByUsernameAsync_ReturnsMatchingUsers()
        {
            _context.Users.Add(new User { UserId = 1, Username = "AliceUser", Password = "Pass1", Role = "User" });
            _context.Users.Add(new User { UserId = 2, Username = "BobEmployee", Password = "Pass2", Role = "Employee" });
            _context.SaveChanges();

            var result = await _service.FindUsersByUsernameAsync("Alice");

            Assert.Single(result);
            Assert.Equal("AliceUser", result[0].Username);
        }

        [Fact]
        public async Task AddUserAsync_AddsUser()
        {
            var user = new User { UserId = 0, Username = "NewUser", Password = "Pass", Role = "User" };

            var addedUser = await _service.AddUserAsync(user);

            Assert.NotNull(addedUser);
            Assert.Equal("NewUser", addedUser.Username);

            var dbUser = await _context.Users.FindAsync(addedUser.UserId);
            Assert.NotNull(dbUser);
            Assert.Equal("NewUser", dbUser.Username);
        }

        [Fact]
        public async Task UpdateUserAsync_UpdatesUser()
        {
            var user = new User { UserId = 1, Username = "ExistingUser", Password = "ExistingPass", Role = "User" };
            _context.Users.Add(user);
            _context.SaveChanges();

            var updatedUser = new User { Username = "UpdatedUser", Password = "UpdatedPass", Role = "Employee" };

            var result = await _service.UpdateUserAsync(1, updatedUser);

            Assert.NotNull(result);
            Assert.Equal("UpdatedUser", result.Username);

            var userInDb = await _context.Users.FindAsync(1);
            Assert.NotNull(userInDb);
            Assert.Equal("UpdatedUser", userInDb.Username);
        }

        [Fact]
        public async Task DeleteUserAsync_RemovesUser()
        {
            var user = new User { UserId = 1, Username = "DeleteUser", Password = "DeletePass", Role = "User" };
            _context.Users.Add(user);
            _context.SaveChanges();

            var result = await _service.DeleteUserAsync(1);

            Assert.True(result);

            var deletedUser = await _context.Users.FindAsync(1);
            Assert.Null(deletedUser);
        }
    }
}