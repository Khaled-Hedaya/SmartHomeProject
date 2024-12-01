using Microsoft.AspNetCore.Mvc;
using SmartHomeProject.Common;
using SmartHomeProject.DTOs;
using SmartHomeProject.Models;
using SmartHomeProject.Services;

namespace SmartHomeProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ItemsController : ControllerBase
    {
        private readonly IItemService _itemService;
        private readonly ILogger<ItemsController> _logger;
        private readonly ItemStateValidator _validator;

        public ItemsController(
            IItemService itemService, 
            ILogger<ItemsController> logger,
            ItemStateValidator validator)
        {
            _itemService = itemService;
            _logger = logger;
            _validator = validator;
        }

        // GET: api/items/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<ItemDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<ItemDto>>> GetItem(Guid id)
        {
            var item = await _itemService.GetByIdAsync(id);
            if (item == null)
                return NotFound(ApiResponse<ItemDto>.Error(
                    new List<string> { "Item not found" },
                    StatusCodes.Status404NotFound));

            return Ok(ApiResponse<ItemDto>.Ok(item));
        }

        // GET: api/items/user/{userId}
        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ItemDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<IEnumerable<ItemDto>>>> GetUserItems(Guid userId)
        {
            var items = await _itemService.GetUserItemsAsync(userId);
            return Ok(ApiResponse<IEnumerable<ItemDto>>.Ok(items));
        }

        // POST: api/items
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<ItemDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<ItemDto>>> CreateItem([FromBody] CreateItemRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<ItemDto>.Error(
                    ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList()));

            try
            {
                var item = new Item
                {
                    ProductId = request.ProductId,
                    Name = request.Name,
                    MacIp = request.MacIp,
                    LastVersionNumber = request.LastVersionNumber,
                    IsAssigned = false
                };

                var createdItem = await _itemService.CreateAsync(item);
                var response = ApiResponse<ItemDto>.Ok(createdItem);
                return CreatedAtAction(nameof(GetItem), new { id = createdItem.Id }, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating item");
                return BadRequest(ApiResponse<ItemDto>.Error(
                    new List<string> { "Error creating item" }));
            }
        }

        // PUT: api/items/{id}/state
        [HttpPut("{id}/state")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<object>>> UpdateState(Guid id, UpdateItemStateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<object>.Error(
                    ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList()));

            try
            {
                var item = await _itemService.GetByIdAsync(id);
                if (item == null)
                    return NotFound(ApiResponse<object>.Error(
                        new List<string> { "Item not found" },
                        StatusCodes.Status404NotFound));

                // Add validation before updating state
                await _validator.ValidateStateAsync(id, request.State, request.Value);
                await _itemService.UpdateStateAsync(id, request.State, request.Value);
                return Ok(ApiResponse<object>.Ok(new { message = "State updated successfully" }));
            }
            catch (InvalidStateException ex)
            {
                _logger.LogWarning(ex, "Invalid state update attempt");
                return BadRequest(ApiResponse<object>.Error(
                    new List<string> { ex.Message }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating item state");
                return BadRequest(ApiResponse<object>.Error(
                    new List<string> { "Error updating item state" }));
            }
        }

        // GET: api/items/{id}/state
        [HttpGet("{id}/state")]
        [ProducesResponseType(typeof(ApiResponse<ItemStateDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<ItemStateDto>>> GetItemState(Guid id)
        {
            try
            {
                var item = await _itemService.GetItemWithStatesAsync(id);
                if (item == null)
                    return NotFound(ApiResponse<ItemStateDto>.Error(
                        new List<string> { "Item not found" },
                        StatusCodes.Status404NotFound));

                var stateDto = new ItemStateDto
                {
                    Id = item.Id,
                    Name = item.Name,
                    States = item.Actions.Select(a => new StateInfo
                    {
                        State = a.State,
                        Value = a.Value,
                        UpdatedAt = a.UpdatedAt
                    }).ToList()
                };

                return Ok(ApiResponse<ItemStateDto>.Ok(stateDto));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving item state");
                return BadRequest(ApiResponse<ItemStateDto>.Error(
                    new List<string> { "Error retrieving item state" }));
            }
        }

        // POST: api/items/{id}/assign/user/{userId}
        [HttpPost("{id}/assign/user/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<object>>> AssignToUser(Guid id, Guid userId)
        {
            try
            {
                var item = await _itemService.GetByIdAsync(id);
                if (item == null)
                    return NotFound(ApiResponse<object>.Error(
                        new List<string> { "Item not found" },
                        StatusCodes.Status404NotFound));

                await _itemService.AssignToUserAsync(id, userId);
                return Ok(ApiResponse<object>.Ok(new { message = "Item assigned to user successfully" }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning item to user");
                return BadRequest(ApiResponse<object>.Error(
                    new List<string> { "Error assigning item to user" }));
            }
        }

        // POST: api/items/{id}/assign/room/{roomId}
        [HttpPost("{id}/assign/room/{roomId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<object>>> AssignToRoom(Guid id, Guid roomId)
        {
            try
            {
                var item = await _itemService.GetByIdAsync(id);
                if (item == null)
                    return NotFound(ApiResponse<object>.Error(
                        new List<string> { "Item not found" },
                        StatusCodes.Status404NotFound));

                await _itemService.AssignToRoomAsync(id, roomId);
                return Ok(ApiResponse<object>.Ok(new { message = "Item assigned to room successfully" }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning item to room");
                return BadRequest(ApiResponse<object>.Error(
                    new List<string> { "Error assigning item to room" }));
            }
        }
    }
}