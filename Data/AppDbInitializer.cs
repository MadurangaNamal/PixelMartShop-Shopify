using Microsoft.AspNetCore.Identity;
using PixelMartShop.Models;

namespace PixelMartShop.Data;

public class AppDbInitializer
{
    public static async Task SeedRolesToDb(IApplicationBuilder applicationBuilder)
    {
        using var serviceScope = applicationBuilder.ApplicationServices.CreateScope();
        var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
            await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));

        if (!await roleManager.RoleExistsAsync(UserRoles.User))
            await roleManager.CreateAsync(new IdentityRole(UserRoles.User));

        if (!await roleManager.RoleExistsAsync(UserRoles.Guest))
            await roleManager.CreateAsync(new IdentityRole(UserRoles.Guest));
    }
}
