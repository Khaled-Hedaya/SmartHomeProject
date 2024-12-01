using Microsoft.EntityFrameworkCore;
using SmartHomeProject.Data;
using SmartHomeProject.DTOs;
using SmartHomeProject.Models;

namespace SmartHomeProject.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserService> _logger;

        private readonly IPasswordHasher _passwordHasher;

        public UserService(ApplicationDbContext context, ILogger<UserService> logger, IPasswordHasher passwordHasher)
        {
            _context = context;
            _logger = logger;
            _passwordHasher = passwordHasher;
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            var users = await _context.Users.ToListAsync();
            return users.Select(MapToDto);
        }

        public async Task<UserDto> GetByIdAsync(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            return user != null ? MapToDto(user) : null;
        }

        public async Task<UserDto> CreateAsync(User user)
        {
            user.Password = _passwordHasher.HashPassword(user.Password);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return MapToDto(user);
        }

        public async Task UpdateAsync(User user)
        {
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<User> GetOriginalUserAsync(Guid id)
        {
            return await _context.Users.FindAsync(id);
        }

        private static UserDto MapToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Phone = user.Phone,
                Image = user.Image,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }
    }
}