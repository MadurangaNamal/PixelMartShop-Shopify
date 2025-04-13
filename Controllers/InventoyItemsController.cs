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
        _inventoryItemService = inventoryItemService;
    }

    //[HttpGet]
    //public async Task<IActionResult> GetInventoryItems()
    //{
    //    var items = await _inventoryItemService.GetAsync(0);
    //    return Ok(items);
    //}
}
