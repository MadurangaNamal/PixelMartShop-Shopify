using PixelMartShop.Entities;

namespace PixelMartShop.Services;

public interface IPixelMartShopRepository
{
    Task<Product> GetProductByIdAsync(long productId);
    Task SaveShopifyProductAsync(Product product);
    Task UpdateShopifyProductAsync(Product product);
    Task<bool> SaveAsync();
}
