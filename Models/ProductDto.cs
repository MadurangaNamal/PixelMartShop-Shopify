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

    public IEnumerable<ProductVariant?> Variants { get; set; }

}
