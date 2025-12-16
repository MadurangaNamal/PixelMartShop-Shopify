using Microsoft.AspNetCore.Mvc;
using ShopifySharp;

namespace PixelMartShop.Controllers;

[Route("api/inventories")]
[ApiController]
public class InventoyItemsController : ControllerBase
{
    private readonly InventoryItemService _inventoryItemService;

    public InventoyItemsController(InventoryItemService inventoryItemService)
    {
        _inventoryItemService = inventoryItemService ?? throw new ArgumentNullException(nameof(inventoryItemService));
    }

    [HttpGet("itemId")]
    public async Task<IActionResult> GetInventoryItem([FromRoute] string itemId)
    {
        var item = await _inventoryItemService.GetAsync(long.Parse(itemId));
        return Ok(item);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllInventoryItems()
    {
        var item = await _inventoryItemService.GetAsync(0);
        return Ok(item);
    }
}
