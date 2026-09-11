using Microsoft.EntityFrameworkCore;
using UserCurrencyApi.Contracts.Currencies;
using UserCurrencyApi.Infrastructure.Data;

namespace UserCurrencyApi.Application.Currencies.Queries;

public sealed record GetCurrenciesQuery;

public sealed class GetCurrenciesQueryHandler(IDbContextFactory<AppDbContext> dbContextFactory)
{
    public async Task<IReadOnlyList<CurrencyResponse>> HandleAsync(CancellationToken cancellationToken)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        return await dbContext.Currencies
            .AsNoTracking()
            .OrderBy(currency => currency.Code)
            .Select(currency => new CurrencyResponse(
                currency.Id,
                currency.Code,
                currency.Name,
                currency.RateToBase))
            .ToArrayAsync(cancellationToken);
    }
}
