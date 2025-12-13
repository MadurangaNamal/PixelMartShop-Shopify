using System.ComponentModel.DataAnnotations;

namespace PixelMartShop.Entities;

public class Product
{
    [Key]
    public long Id { get; set; }

    [Required]
    [MaxLength(250)]
    public required string Title { get; set; }

    [Required]
    public required string BodyHtml { get; set; }

    [MaxLength(100)]
    public string? ProductType { get; set; }

    public IEnumerable<ProductVariant>? Variants { get; set; }
    public IEnumerable<ProductOption>? Options { get; set; }
}
