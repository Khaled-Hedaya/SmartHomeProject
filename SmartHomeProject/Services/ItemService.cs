using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SmartHomeProject.DTOs;
using SmartHomeProject.Hubs;
using SmartHomeProject.Models;

namespace SmartHomeProject.Services
{
    public class ItemService : IItemService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<SmartHomeHub> _hubContext;

        public ItemService(ApplicationDbContext context, IHubContext<SmartHomeHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public async Task<ItemStateDto> GetItemStateAsync(Guid id)
        {
            var item = await _context.Items
                .Include(i => i.Actions)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (item == null)
                return null;

            return new ItemStateDto
            {
                Id = item.Id,
                Name = item.Name,
                States = item.Actions.Select(a => new ItemActionDto
                {
                    State = a.State,
                    Value = a.Value,
                    UpdatedAt = a.UpdatedAt
                }).ToList()
            };
        }

        public async Task<Item> GetByIdAsync(Guid id)
        {
            return await _context.Items
                .Include(i => i.Actions)
                .Include(i => i.Product)
                .Include(i => i.Room)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<IEnumerable<Item>> GetUserItemsAsync(Guid userId)
        {
            return await _context.Items
                .Include(i => i.Actions)
                .Include(i => i.Product)
                .Include(i => i.Room)
                .Where(i => i.UserId == userId)
                .ToListAsync();
        }

        public async Task<Item> CreateAsync(Item item)
        {
            _context.Items.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task UpdateStateAsync(Guid itemId, string state, int value)
        {
            var itemAction = await _context.ItemActions
                .FirstOrDefaultAsync(ia => ia.ItemId == itemId && ia.State == state);

            if (itemAction == null)
            {
                itemAction = new ItemAction
                {
                    ItemId = itemId,
                    State = state,
                    Value = value
                };
                _context.ItemActions.Add(itemAction);
            }
            else
            {
                itemAction.Value = value;
            }

            await _context.SaveChangesAsync();
            await _hubContext.Clients.All.SendAsync("ItemStateChanged", itemId, state, value);
        }

        public async Task AssignToUserAsync(Guid itemId, Guid userId)
        {
            var item = await _context.Items.FindAsync(itemId);
            if (item != null)
            {
                item.UserId = userId;
                item.IsAssigned = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task AssignToRoomAsync(Guid itemId, Guid roomId)
        {
            var item = await _context.Items.FindAsync(itemId);
            if (item != null)
            {
                item.RoomId = roomId;
                await _context.SaveChangesAsync();
            }
        }
    }
}
