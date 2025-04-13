using System.ComponentModel.DataAnnotations;

namespace PixelMartShop.Models;

public class LoginVM
{
    [Required]
    public string EmailAddress { get; set; }

    [Required]
    public string Password { get; set; }
}
