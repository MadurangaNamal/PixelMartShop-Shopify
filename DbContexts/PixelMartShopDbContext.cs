using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PixelMartShop.Entities;

namespace PixelMartShop.DbContexts;

public class PixelMartShopDbContext : IdentityDbContext<ApplicationUser>
{
    public PixelMartShopDbContext(DbContextOptions<PixelMartShopDbContext> options) : base(options)
    {
    }

    public DbSet<RefreshToken> RefreshTokens { get; set; }

}
