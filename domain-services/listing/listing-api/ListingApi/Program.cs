using ListingApi.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Summit.Platform.WebHostCustomization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ListingContext>(options =>
  options.UseSqlServer(builder.Configuration.GetConnectionString("ListingContext")));

WebApplication app = builder.Build();

var context = app.Services.GetRequiredService<ListingContext>();

app.MigrateDbContext<ListingContext>(async (context, services) =>
{
    context.Database.Migrate();
    await new ListingSeed().SeedAsync(context, app.Environment, app.Services.GetRequiredService<ILogger<ListingSeed>>());
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
