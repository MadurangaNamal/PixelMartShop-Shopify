using PixelMartShop.Entities;
using System.ComponentModel.DataAnnotations;

namespace PixelMartShop.Models;

public class ProductDto
{
    [Required]
    public string Title { get; set; }

    [Required]
    public string BodyHtml { get; set; }

    public string ProductType { get; set; }

    public ICollection<ProductVariant?> Variants { get; set; }
}
