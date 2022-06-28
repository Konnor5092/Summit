using ListingApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ListingApi.Infrastructure;

public class ListingContext : DbContext
{
    public ListingContext(DbContextOptions<ListingContext> options) : base(options) { }

    public DbSet<Listing> Listings { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder
            .Entity<Listing>()
            .ToTable("Listings")
            .HasKey(l => l.Id);

        builder
            .Entity<Listing>()
            .Property(l => l.Type)
            .HasConversion(l => l.ToString(), l => (ListingType)Enum.Parse(typeof(ListingType), l));

    }
}
