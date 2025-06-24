using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PixelMartShop;
using PixelMartShop.DbContexts;
using PixelMartShop.Entities;
using PixelMartShop.Helpers;
using PixelMartShop.Middlewares;
using PixelMartShop.Services;
using Serilog;
using ShopifySharp;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var shopifyUrl = builder.Configuration["Shopify:StoreDomain"];
var shopifyToken = builder.Configuration["Shopify:AccessToken"];

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithEnvironmentName()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddDbContext<PixelMartShopDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PixelMartShopDbContextConnection")));

var tokenValidationParameters = new TokenValidationParameters()
{
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["JWT:Secret"])),

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

//Add Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})

//Add JWT Bearer
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
            return context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new
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
            return context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new
            {
                StatusCode = 401,
                Message = "Unauthorized access. Please provide a valid token."
            }));
        },
        OnForbidden = context =>
        {
            context.Response.StatusCode = 403;
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new
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
    string shopifyStoreDomain = builder.Configuration["Shopify:StoreDomain"];
    string accessToken = builder.Configuration["Shopify:AccessToken"];

    return new InventoryItemService(shopifyStoreDomain, accessToken);
});

builder.Services.AddScoped<IPixelMartShopRepository, PixelMartShopRepository>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(Program));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionMiddleware>();

app.UseSerilogRequestLogging(); //Logs basic request info

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<RequestLoggingMiddleware>();
app.MapControllers();

//Seed the roles to database
AppDbInitializer.SeedRolesToDb(app).Wait();

app.Run();
