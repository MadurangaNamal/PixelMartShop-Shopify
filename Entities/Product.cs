using System.ComponentModel.DataAnnotations;

namespace PixelMartShop.Entities;

public class Product
{
    [Key]
    public long Id { get; set; }

    [Required]
    [MaxLength(250)]
    public string Title { get; set; }

    [Required]
    public string BodyHtml { get; set; }

    [MaxLength(100)]
    public string ProductType { get; set; }

    public ICollection<ProductVariant> Variants { get; set; } = [];
}
