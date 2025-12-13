using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PixelMartShop.Entities;

public class ProductOption
{
    public long? Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int? Position { get; set; }

    public IEnumerable<string> Values { get; set; } = [];

    public long? ProductId { get; set; }

    [ForeignKey("ProductId")]
    [JsonIgnore]
    public Product? Product { get; set; } = null!;
}
