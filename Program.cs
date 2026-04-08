using Project_Selling_Clean_Food.Repository;
using Project_Selling_Clean_Food.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DI
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddScoped<IUsersRepo, UserRepo>();
builder.Services.AddScoped<IProductsRepo, ProductsRepo>();
builder.Services.AddScoped<ICartRepo, CartRepo>();
builder.Services.AddScoped<ICartItemRepo, CartItemRepo>();
builder.Services.AddScoped<IOrdersRepo, OrdersRepo>();
builder.Services.AddScoped<IOrderItemRepo, OrderItemRepo>();
builder.Services.AddScoped<IPaymentTransactionRepo, PaymentTransactionRepo>();
builder.Services.AddScoped<IProductCategoryRepo, ProductCategoryRepo>();
builder.Services.AddScoped<IProductImageRepo, ProductImageRepo>();
builder.Services.AddScoped<IVnPayService, VnPayService>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin() // 🔥 deploy thì nên mở hết (sau này tighten lại)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();


// 🔥 QUAN TRỌNG NHẤT - FIX PORT CHO RAILWAY
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Urls.Add($"http://*:{port}");


// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();

app.UseCors("AllowAll");

// ⚠️ Railway không cần HTTPS redirect
// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();