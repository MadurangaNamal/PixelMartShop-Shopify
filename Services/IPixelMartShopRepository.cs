using PixelMartShop.Entities;

namespace PixelMartShop.Services;

public interface IPixelMartShopRepository
{
    Task SaveShopifyProduct(Product product);
    Task UpdateShopifyProduct(Product product);
    Task<bool> SaveAsync();
}
