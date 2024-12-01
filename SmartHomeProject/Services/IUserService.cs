using SmartHomeProject.DTOs;
using SmartHomeProject.Models;

namespace SmartHomeProject.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllAsync();
        Task<UserDto> GetByIdAsync(Guid id);
        Task<UserDto> CreateAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(Guid id);
        Task<User> GetOriginalUserAsync(Guid id);
    }
}