using Project_Selling_Clean_Food.Repository;
using Project_Selling_Clean_Food.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("http://127.0.0.1:5500") // địa chỉ frontend
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();

app.UseCors("AllowAll");

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
