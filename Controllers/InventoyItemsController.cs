using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PixelMartShop.Models;
using ShopifySharp;
using ShopifySharp.Filters;

namespace PixelMartShop.Controllers;

[Authorize(Roles = UserRoles.Admin)]
[Route("api/inventories")]
[ApiController]
public class InventoyItemsController : ControllerBase
{
    private readonly InventoryItemService _inventoryItemService;
    private readonly InventoryLevelService _inventoryLevelService;

    public InventoyItemsController(InventoryItemService inventoryItemService, InventoryLevelService inventoryLevelService)
    {
        _inventoryItemService = inventoryItemService ?? throw new ArgumentNullException(nameof(inventoryItemService));
        _inventoryLevelService = inventoryLevelService ?? throw new ArgumentNullException(nameof(inventoryLevelService));
    }

    [HttpGet("{itemId:long}")]
    public async Task<IActionResult> GetInventoryItem(long itemId)
    {
        var item = await _inventoryItemService.GetAsync(itemId);

        if (item == null)
            return NotFound();

        // Get inventory levels for this inventory item
        var levels = await _inventoryLevelService.ListAsync(
            new InventoryLevelListFilter
            {
                InventoryItemIds = new[] { itemId }
            });

        return Ok(new
        {
            InventoryItem = item,
            InventoryLevels = levels
        });
    }
}
