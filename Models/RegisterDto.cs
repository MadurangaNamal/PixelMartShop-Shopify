using System.ComponentModel.DataAnnotations;

namespace PixelMartShop.Models;

public class RegisterDto
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;

    [Required]
    public required string EmailAddress { get; set; }

    [Required]
    public required string UserName { get; set; }

    [Required]
    public required string Password { get; set; }

    [Required]
    public required string Role { get; set; }
}
