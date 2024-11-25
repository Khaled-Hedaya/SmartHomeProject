namespace SmartHomeProject.Hubs
{
    using Microsoft.AspNetCore.SignalR;
    using SmartHomeProject.Services;

    public class SmartHomeHub : Hub
    {
        private readonly IItemService _itemService;

        public SmartHomeHub(IItemService itemService)
        {
            _itemService = itemService;
        }

        public async Task UpdateItemState(Guid itemId, string state, int value)
        {
            await _itemService.UpdateStateAsync(itemId, state, value);
        }

        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"Client connected: {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            Console.WriteLine($"Client disconnected: {Context.ConnectionId}");
            await base.OnDisconnectedAsync(exception);
        }
    }
}
