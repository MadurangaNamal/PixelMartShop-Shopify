using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PixelMartShop.Data;
using PixelMartShop.Entities;
using PixelMartShop.Middlewares;
using PixelMartShop.Services;
using Serilog;
using ShopifySharp;
using System.Text;
using System.Text.Json;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets<Program>();

var rawConnectionString = builder.Configuration.GetConnectionString("PixelMartShopDbContextConnection")
           ?? throw new InvalidOperationException("Connection string 'PixelMartShopDbContextConnection' not found.");

var dbPassword = builder.Configuration["DB_PASSWORD"]
    ?? throw new InvalidOperationException("Database password 'DB_PASSWORD' not found in configuration.");

var connectionString = rawConnectionString.Replace("{DB_PASSWORD}", dbPassword);

var jwtSecret = builder.Configuration["JWT_SECRET_KEY"]
            ?? throw new InvalidOperationException("JWT secret key not found in configuration.");

var shopifyUrl = builder.Configuration["Shopify:StoreDomain"];
var shopifyToken = builder.Configuration["Shopify_AccessToken"];

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithEnvironmentName()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddDbContext<PixelMartShopDbContext>(options =>
    options.UseNpgsql(connectionString));

var tokenValidationParameters = new TokenValidationParameters()
{
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSecret)),

    ValidateIssuer = true,
    ValidIssuer = builder.Configuration["JWT:Issuer"],

    ValidateAudience = true,
    ValidAudience = builder.Configuration["JWT:Audience"],

    ValidateLifetime = true,
    ClockSkew = TimeSpan.Zero
};

builder.Services.AddSingleton(tokenValidationParameters);

//Add Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<PixelMartShopDbContext>()
    .AddDefaultTokenProviders();

// Add Rate Limiter
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
    {
        var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: ipAddress,
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            });
    });

    options.RejectionStatusCode = 429;
});

//Add Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})  // Add JWT Bearer
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = tokenValidationParameters;

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            context.NoResult();
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                StatusCode = 401,
                Message = "Authentication failed. Token is invalid or expired."
            }));
        },
        OnChallenge = context =>
        {
            context.HandleResponse();
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                StatusCode = 401,
                Message = "Unauthorized access. Please provide a valid token."
            }));
        },
        OnForbidden = context =>
        {
            context.Response.StatusCode = 403;
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                StatusCode = 403,
                Message = "Forbidden. You do not have permission to access this resource."
            }));
        }
    };
});

builder.Services.AddHttpClient();

builder.Services.AddScoped(provider =>
{
    string shopifyStoreDomain = shopifyUrl!;
    string accessToken = shopifyToken!;

    return new ProductService(shopifyStoreDomain, accessToken);
});

builder.Services.AddScoped(provider =>
{
    string shopifyStoreDomain = shopifyUrl!;
    string accessToken = shopifyToken!;

    return new OrderService(shopifyStoreDomain, accessToken);
});

builder.Services.AddScoped(provider =>
{
    string shopifyStoreDomain = shopifyUrl!;
    string accessToken = shopifyToken!;

    return new InventoryItemService(shopifyStoreDomain, accessToken);
});

builder.Services.AddScoped<IPixelMartShopRepository, PixelMartShopRepository>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(Program));

var app = builder.Build();

app.UseSerilogRequestLogging(); //Logs basic request info

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseGlobalExceptionHandler();
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<RequestLoggingMiddleware>();
app.MapControllers();

AppDbInitializer.SeedRolesToDb(app).Wait(); // Seed the roles to db

await app.RunAsync();
