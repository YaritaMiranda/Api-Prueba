using Microsoft.EntityFrameworkCore;
using UserCurrencyApi.Domain.Entities;

namespace UserCurrencyApi.Infrastructure.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<Currency> Currencies => Set<Currency>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(user => user.Id);
            entity.Property(user => user.Name).IsRequired().HasMaxLength(120);
            entity.Property(user => user.Email).IsRequired().HasMaxLength(256);
            entity.Property(user => user.IsActive).HasDefaultValue(true);
            entity.HasIndex(user => user.Email).IsUnique();

            entity.HasMany(user => user.Addresses)
                .WithOne(address => address.User)
                .HasForeignKey(address => address.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Address>(entity =>
        {
            entity.ToTable("Addresses");
            entity.HasKey(address => address.Id);
            entity.Property(address => address.Street).IsRequired().HasMaxLength(180);
            entity.Property(address => address.City).IsRequired().HasMaxLength(120);
            entity.Property(address => address.Country).IsRequired().HasMaxLength(120);
            entity.Property(address => address.ZipCode).HasMaxLength(30);
        });

        modelBuilder.Entity<Currency>(entity =>
        {
            entity.ToTable("Currencies");
            entity.HasKey(currency => currency.Id);
            entity.Property(currency => currency.Code).IsRequired().HasMaxLength(3);
            entity.Property(currency => currency.Name).IsRequired().HasMaxLength(120);
            entity.Property(currency => currency.RateToBase).IsRequired().HasColumnType("decimal(18,6)");
            entity.HasIndex(currency => currency.Code).IsUnique();
        });
    }
}
