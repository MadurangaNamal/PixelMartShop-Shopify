﻿namespace PixelMartShop.Models;

public class AuthResultVM
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public DateTime ExpiresAt { get; set; }
}
