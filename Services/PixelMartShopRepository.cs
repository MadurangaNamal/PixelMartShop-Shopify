using PixelMartShop.DbContexts;
using PixelMartShop.Entities;

namespace PixelMartShop.Services;

public class PixelMartShopRepository : IPixelMartShopRepository
{
    private readonly PixelMartShopDbContext _dbContext;

    public PixelMartShopRepository(PixelMartShopDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task SaveShopifyProduct(Product product)
    {
        ArgumentNullException.ThrowIfNull(product);
        await _dbContext.Products.AddAsync(product);
    }

    public async Task UpdateShopifyProduct(Product product)
    {
        ArgumentNullException.ThrowIfNull(product);
        _dbContext.Products.Update(product);
        await SaveAsync();
    }

    public async Task<bool> SaveAsync()
    {
        return await _dbContext.SaveChangesAsync() >= 0;
    }
}
