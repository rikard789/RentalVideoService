using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideoRentalService.Controllers;
using VideoRentalService.DBContext;
using VideoRentalService.Models;
using VideoRentalService.Services;
using Xunit;

namespace test.Controllers
{
    public class UserControllerTests
    {
        private UsersController _controller;
        private UserService _userService;
        private VideoRentalServiceContext _context;

        public UserControllerTests()
        {
            var options = new DbContextOptionsBuilder<VideoRentalServiceContext>()
                .UseInMemoryDatabase(databaseName: "TestUserDatabase")
                .Options;

            _context = new VideoRentalServiceContext(options);

            removeUsers();

            _userService = new UserService(_context);
            _controller = new UsersController(_userService);
        }

        [Fact]
        public async Task GetAllUsers_Success()
        {
            removeUsers();

            var user = new User { UserId = 1, Username = "Test", Password = "Test", Role = "User" };
            _context.Users.Add(user);
            _context.SaveChanges();

            var result = await _controller.GetAllUsers();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var users = Assert.IsType<List<User>>(okResult.Value);
            Assert.Single(users);
        }

        [Fact]
        public async Task GetAllUsers_Empty()
        {
            removeUsers();

            var result = await _controller.GetAllUsers();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var users = Assert.IsType<List<User>>(okResult.Value);
            Assert.Empty(users);
        }

        [Fact]
        public async Task GetUserById_Success()
        {
            removeUsers();

            var user = new User { UserId = 1, Username = "Test", Password = "Test", Role = "User" };
            _context.Users.Add(user);
            _context.SaveChanges();

            var result = await _controller.GetUserById(1);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var resultUser = Assert.IsType<User>(okResult.Value);
            Assert.Equal(resultUser.Username, user.Username);
        }

        [Fact]
        public async Task GetUserById_NotFound()
        {
            var result = await _controller.GetUserById(999);

            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetUserByUsername_Success()
        {
            removeUsers();

            var user = new User { UserId = 1, Username = "Test", Password = "Test", Role = "User" };
            _context.Users.Add(user);
            _context.SaveChanges();

            var result = await _controller.GetUserByUsername("Test");

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var resultUsers = Assert.IsType<List<User>>(okResult.Value);
            Assert.Single(resultUsers);
        }

        [Fact]
        public async Task GetUserByUsername_NotFound()
        {
            var result = await _controller.GetUserByUsername("User123");

            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task CreateUser_Success()
        {
            removeUsers();

            var user = new User { UserId = 1, Username = "Test", Password = "Test", Role = "User" };

            var result = await _controller.CreateUser(user);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedUser = Assert.IsType<User>(createdResult.Value);
            Assert.Equal(user.Username, returnedUser.Username);
        }

        [Fact]
        public async Task UpdateUser_Success()
        {
            removeUsers();

            var user = new User { UserId = 1, Username = "Test", Password = "Test", Role = "User" };
            _context.Users.Add(user);
            _context.SaveChanges();

            var updatedUser = new User { UserId = 2, Username = "Test Test", Password = "Test Test", Role = "Admin" };

            var result = await _controller.UpdateUser(1, updatedUser);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedUser = Assert.IsType<User>(okResult.Value);
            Assert.Equal(updatedUser.Username, returnedUser.Username);
        }

        [Fact]
        public async Task UpdateUser_NotFound()
        {
            removeUsers();

            var updatedUser = new User { UserId = 1, Username = "Test", Password = "Test", Role = "User" };

            var result = await _controller.UpdateUser(99, updatedUser);

            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task DeleteUser_Success()
        {
            removeUsers();

            var user = new User { UserId = 1, Username = "Test", Password = "Test", Role = "User" };
            _context.Users.Add(user);
            _context.SaveChanges();

            var result = await _controller.DeleteUser(1);

            Assert.IsType<NoContentResult>(result);
            Assert.Null(await _context.Users.FindAsync(1));
        }

        [Fact]
        public async Task DeleteUser_NotFound()
        {
            removeUsers();

            var result = await _controller.DeleteUser(99);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        private void removeUsers()
        {
            _context.Users.RemoveRange(_context.Users);
            _context.SaveChanges();
        }
    }
}
