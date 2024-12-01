using Microsoft.AspNetCore.SignalR;
using SmartHomeProject.Common.Exceptions;
using SmartHomeProject.Services;

namespace SmartHomeProject.Hubs
{
    public class SmartHomeHub : Hub
    {
        private readonly IItemService _itemService;
        private readonly ILogger<SmartHomeHub> _logger;
        private readonly ItemStateValidator _validator;

        public SmartHomeHub(IItemService itemService, ILogger<SmartHomeHub> logger, ItemStateValidator validator)
        {
            _itemService = itemService;
            _logger = logger;
            _validator = validator;
        }

        public async Task UpdateItemState(string itemId, string state, string value)
        {
            try
            {
                // Validate state first
                await _validator.ValidateStateAsync(Guid.Parse(itemId), state, value);
                
                // Get item for name and update state
                var item = await _itemService.GetByIdAsync(Guid.Parse(itemId));
                if (item == null) throw new NotFoundException("Item not found");

                await _itemService.UpdateStateAsync(Guid.Parse(itemId), state, value);
                
                // Broadcast to all clients
                await Clients.Group("AllClients").SendAsync("ReceiveStateUpdate", new
                {
                    itemId,
                    itemName = item.Name,  // Include name for UI purposes
                    state,
                    value,
                    timestamp = DateTime.UtcNow
                });
                
                _logger.LogInformation(
                    "State updated for Item: {ItemName}({ItemId}), State: {State}, Value: {Value}", 
                    item.Name, itemId, state, value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, 
                    "Error updating state through SignalR - ItemId: {ItemId}, State: {State}, Value: {Value}", 
                    itemId, state, value);
                await Clients.Caller.SendAsync("StateUpdateError", ex.Message);
                throw;
            }
        }

        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "AllClients");
            _logger.LogInformation("Client connected: {ConnectionId}", Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            _logger.LogInformation(
                exception,
                "Client disconnected: {ConnectionId}",
                Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
    }
}