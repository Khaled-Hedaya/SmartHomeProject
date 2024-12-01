using Microsoft.EntityFrameworkCore;
using SmartHomeProject.Data;

public class ItemStateValidator
{
    private readonly ApplicationDbContext _context;

    public ItemStateValidator(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task ValidateStateAsync(Guid itemId, string state, string value)
    {
        var productAction = await _context.Items
            .Where(i => i.Id == itemId)
            .Select(i => i.Product.Actions.FirstOrDefault(pa => pa.State == state))
            .FirstOrDefaultAsync();

        if (productAction == null)
            throw new InvalidStateException($"State '{state}' is not allowed for this product");
    }
}