using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<StoreContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")); //'DefaultConnection' same as in appsettings.Development.json setting
});

//!all staff before is a SERVICE, serive is sth that we inject to classes in our application
var app = builder.Build();
//!all staff after is a MIDDLEWARES
// Configure the HTTP request pipeline

app.MapControllers();

app.Run();
