using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PixelMartShop.Models;
using PixelMartShop.Services;
using ShopifySharp;

namespace PixelMartShop.Controllers;

[Authorize]
[Obsolete]
[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IPixelMartShopRepository _pixelMartShopRepository;
    private readonly ProductService _productService;

    public ProductsController(ProductService productService, IMapper mapper, IPixelMartShopRepository pixelMartShopRepository)
    {
        _productService = productService ?? throw new ArgumentNullException(nameof(productService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _pixelMartShopRepository = pixelMartShopRepository
            ?? throw new ArgumentNullException(nameof(pixelMartShopRepository));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllProducts()
    {
        var products = await _productService.ListAsync();
        return Ok(products);
    }

    [HttpGet("{id}", Name = "GetProductById")]
    public async Task<IActionResult> GetProductById([FromRoute] long id)
    {
        var product = await _productService.GetAsync(id);

        if (product == null)
            return NotFound("Product Not Found");

        return Ok(product);
    }

    [HttpPost(Name = "CreateProduct")]
    public async Task<IActionResult> CreateProduct([FromBody] ProductDto productDto)
    {
        ArgumentNullException.ThrowIfNull(productDto);

        var productEntity = _mapper.Map<Entities.Product>(productDto);
        var productForShopify = _mapper.Map<Product>(productEntity);

        var newProduct = await _productService.CreateAsync(productForShopify);

        if (newProduct?.Id == null)
            return BadRequest("Failed to create Product");

        var productForRepository = _mapper.Map<Entities.Product>(newProduct);

        await _pixelMartShopRepository.SaveShopifyProductAsync(productForRepository);
        await _pixelMartShopRepository.SaveAsync();

        return CreatedAtRoute("GetProductById", new { id = productForRepository.Id }, productForRepository);
    }

    [HttpPut("{productId}", Name = "UpdateProduct")]
    public async Task<IActionResult> UpdateProduct([FromRoute] long productId, [FromBody] ProductDto productDto)
    {
        ArgumentNullException.ThrowIfNull(productDto);

        var existingShopifyProduct = await _productService.GetAsync(productId);

        if (existingShopifyProduct == null)
            return NotFound("Product Not Found");

        var productEntity = _mapper.Map<Entities.Product>(productDto);
        _mapper.Map(productEntity, existingShopifyProduct);

        var updatedProduct = await _productService.UpdateAsync(productId, existingShopifyProduct);

        if (updatedProduct == null)
            return BadRequest("Failed to update shopify Product");

        if (updatedProduct.Status == "active")
        {
            var updatedProductForTheRepository = _mapper.Map<Entities.Product>(updatedProduct);
            var existingProductInRepository = await _pixelMartShopRepository.GetProductByIdAsync(productId);

            if (existingProductInRepository == null)
                return NotFound($"Product with id:{productId} Not Found");

            _mapper.Map(updatedProductForTheRepository, existingProductInRepository);

            await _pixelMartShopRepository.UpdateShopifyProductAsync(existingProductInRepository);
        }

        return NoContent();
    }
}
