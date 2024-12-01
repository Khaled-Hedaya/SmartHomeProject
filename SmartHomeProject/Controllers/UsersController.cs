using Microsoft.AspNetCore.Mvc;
using SmartHomeProject.Common;
using SmartHomeProject.DTOs;
using SmartHomeProject.Models;
using SmartHomeProject.Services;

namespace SmartHomeProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        // GET: api/users
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<UserDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<IEnumerable<UserDto>>>> GetUsers()
        {
            var users = await _userService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<UserDto>>.Ok(users));
        }

        // GET: api/users/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<UserDto>>> GetUser(Guid id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
                return NotFound(ApiResponse<UserDto>.Error(
                    new List<string> { "User not found" },
                    StatusCodes.Status404NotFound));

            return Ok(ApiResponse<UserDto>.Ok(user));
        }

        // POST: api/users
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<UserDto>>> CreateUser(CreateUserRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<UserDto>.Error(
                    ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList()));

            try
            {
                var user = new User
                {
                    Username = request.Username,
                    Email = request.Email,
                    Phone = request.Phone,
                    Password = request.Password,
                    Image = request.Image
                };

                var createdUser = await _userService.CreateAsync(user);
                var response = ApiResponse<UserDto>.Ok(createdUser);
                return CreatedAtAction(
                    nameof(GetUser),
                    new { id = createdUser.Id },
                    response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                return BadRequest(ApiResponse<UserDto>.Error(
                    new List<string> { "Error creating user" }));
            }
        }

        // PUT: api/users/{id}
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateUser(Guid id, UpdateUserRequest request)
    {
    if (!ModelState.IsValid)
        return BadRequest(ApiResponse<object>.Error(
            ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList()));

        try
        {
            var existingUser = await _userService.GetByIdAsync(id);
            if (existingUser == null)
                return NotFound(ApiResponse<object>.Error(
                    new List<string> { "User not found" },
                    StatusCodes.Status404NotFound));

            // Get the original user from the database through the service
            var originalUser = await _userService.GetOriginalUserAsync(id);
            if (originalUser == null)
                return NotFound(ApiResponse<object>.Error(
                    new List<string> { "User not found" },
                    StatusCodes.Status404NotFound));

            // Map the update request to the existing user
            var user = new User
            {
                Id = id,
                Username = request.Username,
                Email = request.Email,
                Phone = request.Phone,
                Image = request.Image,
                Password = originalUser.Password, // Use the password from original user
                CreatedAt = originalUser.CreatedAt,
                UpdatedAt = DateTime.UtcNow
            };

            await _userService.UpdateAsync(user);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user");
            return BadRequest(ApiResponse<object>.Error(
                new List<string> { "Error updating user" }));
        }
    }

        // DELETE: api/users/{id}
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteUser(Guid id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
                return NotFound(ApiResponse<object>.Error(
                    new List<string> { "User not found" },
                    StatusCodes.Status404NotFound));

            await _userService.DeleteAsync(id);
            return NoContent();
        }
    }
}