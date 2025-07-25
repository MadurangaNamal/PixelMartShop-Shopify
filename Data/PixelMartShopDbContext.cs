﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PixelMartShop.Entities;

namespace PixelMartShop.Data;

public class PixelMartShopDbContext : IdentityDbContext<ApplicationUser>
{
    public PixelMartShopDbContext(DbContextOptions<PixelMartShopDbContext> options)
        : base(options)
    {
    }

    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductVariant> ProductVariants { get; set; }
}
