﻿using System.ComponentModel.DataAnnotations;

namespace PixelMartShop.Models;

public class TokenRequestVM
{
    [Required]
    public string Token { get; set; }

    [Required]
    public string RefreshToken { get; set; }
}
