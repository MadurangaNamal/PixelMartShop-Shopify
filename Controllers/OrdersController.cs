using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PixelMartShop.Models;
using ShopifySharp;

namespace PixelMartShop.Controllers;

[Authorize(Roles = UserRoles.Admin)]
[Route("api/orders")]
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly OrderService _orderService;

    public OrdersController(OrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllOrders()
    {
        var orders = await _orderService.ListAsync();
        return Ok(orders);
    }
}
