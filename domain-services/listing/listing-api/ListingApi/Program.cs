using ListingApi.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Summit.Platform.WebHostCustomization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.SuppressAsyncSuffixInActionNames = false;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ListingContext>(options =>
  options.UseSqlServer(builder.Configuration.GetConnectionString("ListingContext")));

var app = builder.Build();

app.MigrateDbContext<ListingContext>((context, services) =>
{
    new ListingSeed().SeedAsync(context, app.Environment, app.Services.GetRequiredService<ILogger<ListingSeed>>()).Wait();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
