using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PixelMartShop.Entities;

public class ProductVariant
{
    public long? Id { get; set; }
    public string SKU { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal? Price { get; set; }

    public string Barcode { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal? Weight { get; set; }

    public string WeightUnit { get; set; } = string.Empty;

    public string? Option1 { get; set; }
    public string? Option2 { get; set; }

    public long? ProductId { get; set; }

    [ForeignKey("ProductId")]
    [JsonIgnore]
    public Product? Product { get; set; } = null!;
}
