using System.ComponentModel.DataAnnotations;

namespace PixelMartShop.Models;

public class TokenRequestDto
{
    [Required]
    public string Token { get; set; }

    [Required]
    public string RefreshToken { get; set; }
}
