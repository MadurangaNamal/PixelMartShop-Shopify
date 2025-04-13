using Microsoft.AspNetCore.Mvc;
using ShopifySharp;

namespace PixelMartShop.Controllers;

[Route("api/products")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly ProductService _productService;

    public ProductController(ProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts()
    {
        var products = await _productService.ListAsync();
        return Ok(products);
    }
}
