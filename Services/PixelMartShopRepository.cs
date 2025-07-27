using Microsoft.EntityFrameworkCore;
using PixelMartShop.Data;
using PixelMartShop.Entities;

namespace PixelMartShop.Services;

public class PixelMartShopRepository : IPixelMartShopRepository
{
    private readonly PixelMartShopDbContext _dbContext;

    public PixelMartShopRepository(PixelMartShopDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public Task<Product> GetProductByIdAsync(long productId)
    {
        var product = _dbContext.Products.Include(p => p.Variants).FirstOrDefaultAsync(p => p.Id == productId);
        return product!;
    }

    public async Task SaveShopifyProductAsync(Product product)
    {
        ArgumentNullException.ThrowIfNull(product);
        await _dbContext.Products.AddAsync(product);
    }

    public async Task UpdateShopifyProductAsync(Product product)
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
