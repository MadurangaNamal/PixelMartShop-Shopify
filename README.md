# PixelMartShop

**PixelMartShop** is an ASP.NET Core Web API application designed to integrate with a Shopify store. This solution ensures seamless synchronisation of products, inventory, and orders between the Shopify store and a PostgreSQL database using Entity Framework Core. The application exposes RESTful endpoints to manage data on Shopify while maintaining a reliable local copy in the database.


## 🚀 Features

- 🔗 **Shopify Integration** — Connects directly with your Shopify store using ShopifySharp or equivalent.
- 🗃️ **PostgreSQL Database Sync** — Data from Shopify is synced with PostgreSQL using Entity Framework Core.
- 🔐 **JWT Authentication & Authorization** — Secure endpoints with token-based access control.
- 📝 **Serilog Logging** — Advanced structured logging with custom middleware for comprehensive request/response tracking.
- 📊 **Comprehensive Logging** — Detailed monitoring with performance metrics, error tracking, and audit trails.
- 🧰 **Clean Architecture** — Separation of concerns using Controllers, Services, Models, and DbContexts.
- 🧾 **Product, Order, and Inventory Management** — Full CRUD capabilities via API endpoints.


## 📁 Project Structure

```plaintext
PixelMartShop/
│
├── Controllers/             # API Controllers for handling authentication, products, orders, and inventory
│   ├── AuthenticationController.cs
│   ├── InventoryItemsController.cs
│   ├── OrdersController.cs
│   └── ProductsController.cs
│
├── DbContexts/              # EF Core DbContext classes
├── Entities/                # Domain entities and models
├── Helpers/                 # Utility and helper classes
├── Middlewares/             # Custom middleware components
├── Migrations/              # EF Core migration files
├── Models/                  # DTOs and request/response models
├── Profiles/                # AutoMapper profiles
├── Services/                # Business logic and Shopify integration logic
│
├── AppDbInitializer.cs      # Database initializer
├── appsettings.json         # Configuration settings
├── PixelMartShop.http       # HTTP request samples for testing
├── Program.cs               # Entry point of the application
└── README.md                # Project documentation
