using Microsoft.AspNetCore.Identity;

namespace PixelMartShop.Entities;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}
