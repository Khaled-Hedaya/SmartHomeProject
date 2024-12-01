using Microsoft.EntityFrameworkCore;
using SmartHomeProject.Common.Exceptions;
using SmartHomeProject.Data;
using SmartHomeProject.DTOs;
using SmartHomeProject.Models;

namespace SmartHomeProject.Services
{
    public class ItemService : IItemService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ItemService> _logger;

        public ItemService(ApplicationDbContext context, ILogger<ItemService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ItemDto> GetByIdAsync(Guid id)
        {
            var item = await _context.Items.FindAsync(id);
            return item != null ? MapToDto(item) : null;
        }

        public async Task<IEnumerable<ItemDto>> GetUserItemsAsync(Guid userId)
        {
            var items = await _context.Items
                .Where(i => i.UserId == userId)
                .ToListAsync();
            return items.Select(MapToDto);
        }

        public async Task<ItemDto> CreateAsync(Item item)
        {
            _context.Items.Add(item);
            await _context.SaveChangesAsync();
            return MapToDto(item);
        }

        public async Task UpdateStateAsync(Guid itemId, string state, string value)
        {
            _logger.LogInformation("Updating state for item {ItemId}. State: {State}, Value: {Value}", 
                itemId, state, value);

            // Get item with its product actions
            var item = await _context.Items
                .Include(i => i.Product)
                .ThenInclude(p => p.Actions)
                .FirstOrDefaultAsync(i => i.Id == itemId);

            if (item == null)
                throw new NotFoundException($"Item with ID {itemId} not found");

            // Validate that the state is allowed for this product
            var allowedState = item.Product.Actions
                .FirstOrDefault(a => a.State.Equals(state, StringComparison.OrdinalIgnoreCase));

            if (allowedState == null)
                throw new InvalidStateException($"State '{state}' is not allowed for product type {item.Product.Name}");

            // Now update or create the item action
            var existingAction = await _context.ItemActions
                .FirstOrDefaultAsync(a => a.ItemId == itemId && a.State == state);

            if (existingAction != null)
            {
                existingAction.Value = value;
                existingAction.UpdatedAt = DateTime.UtcNow;
                _logger.LogDebug("Updated existing state for item {ItemId}", itemId);
            }
            else
            {
                var newAction = new ItemAction
                {
                    ItemId = itemId,
                    State = state,
                    Value = value,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.ItemActions.Add(newAction);
                _logger.LogDebug("Created new state for item {ItemId}", itemId);
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Successfully updated state for item {ItemId}", itemId);
        }

        public async Task<Item> GetItemWithStatesAsync(Guid id)
        {
            return await _context.Items
                .Include(i => i.Actions)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task AssignToUserAsync(Guid itemId, Guid userId)
        {
            var item = await _context.Items.FindAsync(itemId);
            if (item == null)
                throw new KeyNotFoundException($"Item with ID {itemId} not found");

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {userId} not found");

            item.UserId = userId;
            item.IsAssigned = true;
            await _context.SaveChangesAsync();
        }

        public async Task AssignToRoomAsync(Guid itemId, Guid roomId)
        {
            var item = await _context.Items.FindAsync(itemId);
            if (item == null)
                throw new KeyNotFoundException($"Item with ID {itemId} not found");

            var room = await _context.Rooms.FindAsync(roomId);
            if (room == null)
                throw new KeyNotFoundException($"Room with ID {roomId} not found");

            item.RoomId = roomId;
            await _context.SaveChangesAsync();
        }

        private static ItemDto MapToDto(Item item)
        {
            return new ItemDto
            {
                Id = item.Id,
                ProductId = item.ProductId,
                Name = item.Name,
                MacIp = item.MacIp,
                LastVersionNumber = item.LastVersionNumber,
                IsAssigned = item.IsAssigned,
                CreatedAt = item.CreatedAt,
                UpdatedAt = item.UpdatedAt
            };
        }
    }
}