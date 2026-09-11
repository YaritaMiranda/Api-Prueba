using Microsoft.EntityFrameworkCore;
using UserCurrencyApi.Domain.Entities;

namespace UserCurrencyApi.Infrastructure.Data;

public sealed class DatabaseInitializer(IDbContextFactory<AppDbContext> dbContextFactory)
{
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        await dbContext.Database.EnsureCreatedAsync(cancellationToken);

        if (await dbContext.Currencies.AnyAsync(cancellationToken))
        {
            return;
        }

        dbContext.Currencies.AddRange(
            new Currency { Code = "PYG", Name = "Paraguayan Guarani", RateToBase = 1m },
            new Currency { Code = "USD", Name = "US Dollar", RateToBase = 7300m },
            new Currency { Code = "EUR", Name = "Euro", RateToBase = 7900m },
            new Currency { Code = "BRL", Name = "Brazilian Real", RateToBase = 1450m });

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
