using SmartHomeProject.Models;

namespace SmartHomeProject.Services
{
    public interface IUserService
    {
        Task<User> GetByIdAsync(Guid id);
        Task<User> CreateAsync(User user);
        Task<IEnumerable<User>> GetAllAsync();
        Task<User> UpdateAsync(User user);
        Task DeleteAsync(Guid id);
    }
}
