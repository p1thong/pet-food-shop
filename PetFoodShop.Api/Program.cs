using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PayOS;
using PetFoodShop.Api.Data;
using PetFoodShop.Api.Repositories.Implements;
using PetFoodShop.Api.Repositories.Interfaces;
using PetFoodShop.Api.Services;
using PetFoodShop.Api.Services.Implements;
using PetFoodShop.Api.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

var firebase = Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS") ?? "";

FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile(firebase)
});

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//DbContext
builder.Services.AddDbContext<PetFoodShopContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PetFoodShopDb")));

builder.Services.Configure<PayOSOptions>(builder.Configuration.GetSection("PayOS"));

builder.Services.AddSingleton<PayOSClient>(sp =>
{
    var opt = sp.GetRequiredService<IOptions<PayOSOptions>>().Value;

    var clientId    = "791e67b8-2b09-4dbf-b0d3-f0c4558b2f4c";
    var apiKey      = "8fc015de-be14-4e3b-92d1-ef26aedbe269";
    var checksumKey = "9b20998feac661487bba51dde167662e9d7230d252f84a098ec8bdd83f0c328c";

    if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(checksumKey))
        throw new InvalidOperationException("PayOSOptions are missing. Check appsettings or secrets.");

    // IMPORTANT: if your keys are for sandbox, you must point BaseUrl to sandbox; if live, keep prod.
    return new PayOSClient(new PayOSOptions
    {
        ClientId    = clientId,
        ApiKey      = apiKey,
        ChecksumKey = checksumKey,
    });
});


// Register Repositories
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IStoreLocationRepository, StoreLocationRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();

// Register Services
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IStoreLocationService, StoreLocationService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

// Other
builder.Services.AddSingleton<FCMService>();

var app = builder.Build();

app.UseForwardedHeaders();

// Middleware
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();