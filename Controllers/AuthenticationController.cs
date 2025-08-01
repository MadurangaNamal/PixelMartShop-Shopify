﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PixelMartShop.Data;
using PixelMartShop.Entities;
using PixelMartShop.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PixelMartShop.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly PixelMartShopDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly TokenValidationParameters _tokenValidationParameters;

    public AuthenticationController(UserManager<ApplicationUser> userManager,
           RoleManager<IdentityRole> roleManager,
           PixelMartShopDbContext context,
           IConfiguration configuration,
           TokenValidationParameters tokenValidationParameters)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
        _configuration = configuration;
        _tokenValidationParameters = tokenValidationParameters;
    }

    [HttpPost("register-user")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerVM)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Please, provide all the required fields");
        }

        var userExists = await _userManager.FindByEmailAsync(registerVM.EmailAddress);
        if (userExists != null)
        {
            return BadRequest($"User {registerVM.EmailAddress} already exists");
        }

        ApplicationUser newUser = new()
        {
            FirstName = registerVM.FirstName,
            LastName = registerVM.LastName,
            Email = registerVM.EmailAddress,
            UserName = registerVM.UserName,
            SecurityStamp = Guid.NewGuid().ToString()
        };

        var result = await _userManager.CreateAsync(newUser, registerVM.Password);

        if (result.Succeeded)
        {
            //Add user role

            switch (registerVM.Role)
            {
                case UserRoles.Admin:
                    await _userManager.AddToRoleAsync(newUser, UserRoles.Admin);
                    break;
                case UserRoles.User:
                    await _userManager.AddToRoleAsync(newUser, UserRoles.User);
                    break;
                default:
                    break;
            }

            return Ok("User created");
        }

        return BadRequest("User could not be created");
    }

    [HttpPost("login-user")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginVM)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Please, provide all required fields");
        }

        var userExists = await _userManager.FindByEmailAsync(loginVM.EmailAddress);
        if (userExists != null && await _userManager.CheckPasswordAsync(userExists, loginVM.Password))
        {
            var tokenValue = await GenerateJWTTokenAsync(userExists, null);
            return Ok(tokenValue);
        }
        return Unauthorized();
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] TokenRequestDto tokenRequestVM)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Please, provide all required fields");
        }

        var result = await VerifyAndGenerateTokenAsync(tokenRequestVM);
        return Ok(result);
    }

    private async Task<AuthResultDto> VerifyAndGenerateTokenAsync(TokenRequestDto tokenRequestVM)
    {
        var jwtTokenHandler = new JwtSecurityTokenHandler();
        var storedToken = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == tokenRequestVM.RefreshToken);
        var dbUser = await _userManager.FindByIdAsync(storedToken.UserId);

        try
        {
            var tokenCheckResult = jwtTokenHandler.ValidateToken(tokenRequestVM.Token, _tokenValidationParameters, out var validatedToken);

            return await GenerateJWTTokenAsync(dbUser, storedToken);
        }
        catch (SecurityTokenExpiredException)
        {
            if (storedToken.DateExpire >= DateTime.UtcNow)
            {
                return await GenerateJWTTokenAsync(dbUser, storedToken);
            }
            else
            {
                return await GenerateJWTTokenAsync(dbUser, null);
            }
        }
    }

    private async Task<AuthResultDto> GenerateJWTTokenAsync(ApplicationUser user, RefreshToken rToken)
    {
        var authClaims = new List<Claim>()
            {
                new(ClaimTypes.Name, user.UserName),
                new(ClaimTypes.NameIdentifier, user.Id),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new(JwtRegisteredClaimNames.Sub, user.Email),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

        //Add User Role Claims
        var userRoles = await _userManager.GetRolesAsync(user);
        foreach (var userRole in userRoles)
        {
            authClaims.Add(new Claim(ClaimTypes.Role, userRole));
        }


        var authSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]));

        var token = new JwtSecurityToken(
            issuer: _configuration["JWT:Issuer"],
            audience: _configuration["JWT:Audience"],
            expires: DateTime.UtcNow.AddMinutes(50),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));

        var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

        if (rToken != null)
        {
            var rTokenResponse = new AuthResultDto()
            {
                Token = jwtToken,
                RefreshToken = rToken.Token,
                ExpiresAt = token.ValidTo
            };
            return rTokenResponse;
        }

        var refreshToken = new RefreshToken()
        {
            JwtId = token.Id,
            IsRevoked = false,
            UserId = user.Id,
            DateAdded = DateTime.UtcNow,
            DateExpire = DateTime.UtcNow.AddMonths(6),
            Token = Guid.NewGuid().ToString() + "-" + Guid.NewGuid().ToString()
        };
        await _context.RefreshTokens.AddAsync(refreshToken);
        await _context.SaveChangesAsync();


        var response = new AuthResultDto()
        {
            Token = jwtToken,
            RefreshToken = refreshToken.Token,
            ExpiresAt = token.ValidTo
        };

        return response;

    }
}
