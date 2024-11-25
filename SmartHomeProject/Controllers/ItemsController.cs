using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartHomeProject.DTOs;
using SmartHomeProject.Models;
using SmartHomeProject.Services;

namespace SmartHomeProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly IItemService _itemService;
        private readonly ApplicationDbContext _context;

        public ItemsController(IItemService itemService, ApplicationDbContext context)
        {
            _itemService = itemService;
            _context = context;
        }

        // GET: api/items/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Item>> GetItem(Guid id)
        {
            var item = await _itemService.GetByIdAsync(id);
            if (item == null)
                return NotFound();
            return Ok(item);
        }

        // GET: api/items/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Item>>> GetUserItems(Guid userId)
        {
            var items = await _itemService.GetUserItemsAsync(userId);
            return Ok(items);
        }

        // POST: api/items
        [HttpPost]
        public async Task<ActionResult<Item>> CreateItem([FromBody] CreateItemRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var item = new Item
            {
                ProductId = request.ProductId,
                Name = request.Name,
                MacIp = request.MacIp,
                LastVersionNumber = request.LastVersionNumber,
                IsAssigned = false
            };

            var createdItem = await _itemService.CreateAsync(item);
            return CreatedAtAction(nameof(GetItem), new { id = createdItem.Id }, createdItem);
        }

        // PUT: api/items/{id}/state
        [HttpPut("{id}/state")]
        public async Task<IActionResult> UpdateState(Guid id, UpdateItemStateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var item = await _itemService.GetByIdAsync(id);
            if (item == null)
                return NotFound();

            await _itemService.UpdateStateAsync(id, request.State, request.Value);
            return Ok();
        }

        [HttpGet("{id}/state")]
        public async Task<ActionResult<object>> GetItemState(Guid id)
        {
            var item = await _context.Items
                .Include(i => i.Actions)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (item == null)
                return NotFound(new { message = "Item not found" });

            return Ok(new
            {
                item.Id,
                item.Name,
                States = item.Actions.Select(a => new
                {
                    a.State,
                    a.Value,
                    a.UpdatedAt
                }).ToList()
            });
        }

        // POST: api/items/{id}/assign/user/{userId}
        [HttpPost("{id}/assign/user/{userId}")]
        public async Task<IActionResult> AssignToUser(Guid id, Guid userId)
        {
            var item = await _itemService.GetByIdAsync(id);
            if (item == null)
                return NotFound();

            await _itemService.AssignToUserAsync(id, userId);
            return Ok();
        }

        // POST: api/items/{id}/assign/room/{roomId}
        [HttpPost("{id}/assign/room/{roomId}")]
        public async Task<IActionResult> AssignToRoom(Guid id, Guid roomId)
        {
            var item = await _itemService.GetByIdAsync(id);
            if (item == null)
                return NotFound();

            await _itemService.AssignToRoomAsync(id, roomId);
            return Ok();
        }
    }
}
