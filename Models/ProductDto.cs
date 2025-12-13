using PixelMartShop.Entities;
using System.ComponentModel.DataAnnotations;

namespace PixelMartShop.Models;

public class ProductDto
{
    [Required]
    public required string Title { get; set; }

    [Required]
    public required string BodyHtml { get; set; }

    public string? ProductType { get; set; }

    public IEnumerable<ProductVariant>? Variants { get; set; }

    public IEnumerable<ProductOption>? Options { get; set; }
}
