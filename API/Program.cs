using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container. Positiong dosent matter for services

builder.Services.AddControllers();
builder.Services.AddDbContext<StoreContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")); //'DefaultConnection' same as in appsettings.Development.json setting
});
builder.Services.AddScoped<IProductRepository, ProductRepository>();//*AddScoped - okresla jak dlugo ma istniec ten service - addScopped znaczy ze tak dlugo jak http request
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));//*AddScoped - okresla jak dlugo ma istniec ten service - addScopped znaczy ze tak dlugo jak http request

//!all staff before is a SERVICE, serive is sth that we inject to classes in our application
var app = builder.Build();

//!all staff after is a MIDDLEWARES - improtant what is before/after
// Configure the HTTP request pipeline

app.MapControllers();

try
{
    using var scope = app.Services.CreateScope();
    var serivces = scope.ServiceProvider;
    var context = serivces.GetRequiredService<StoreContext>();
    await context.Database.MigrateAsync();
    await StoreContextSeed.SeedAsync(context);
}
catch (System.Exception ex)
{
    Console.WriteLine(ex);
    throw;
}

app.Run();
