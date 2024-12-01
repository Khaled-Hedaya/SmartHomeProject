using SmartHomeProject.DTOs;
using SmartHomeProject.Models;

namespace SmartHomeProject.Services
{
    public interface IItemService
    {
       Task<ItemDto> GetByIdAsync(Guid id);
        Task<IEnumerable<ItemDto>> GetUserItemsAsync(Guid userId);
        Task<ItemDto> CreateAsync(Item item);
        Task<Item> GetItemWithStatesAsync(Guid id);
        Task UpdateStateAsync(Guid itemId, string state, string value);
        Task AssignToUserAsync(Guid itemId, Guid userId);
        Task AssignToRoomAsync(Guid itemId, Guid roomId);
    }
}