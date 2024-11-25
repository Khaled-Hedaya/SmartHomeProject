using SmartHomeProject.DTOs;
using SmartHomeProject.Models;

namespace SmartHomeProject.Services
{
    public interface IItemService
    {
        Task<Item> GetByIdAsync(Guid id);
        Task<IEnumerable<Item>> GetUserItemsAsync(Guid userId);
        Task<Item> CreateAsync(Item item);
        Task UpdateStateAsync(Guid itemId, string state, int value);
        Task AssignToUserAsync(Guid itemId, Guid userId);
        Task AssignToRoomAsync(Guid itemId, Guid roomId);
        Task<ItemStateDto> GetItemStateAsync(Guid id);
    }
}
