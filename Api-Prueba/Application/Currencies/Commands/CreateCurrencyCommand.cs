using Microsoft.EntityFrameworkCore;
using UserCurrencyApi.Application.Common;
using UserCurrencyApi.Contracts.Currencies;
using UserCurrencyApi.Domain.Entities;
using UserCurrencyApi.Infrastructure.Data;

namespace UserCurrencyApi.Application.Currencies.Commands;

public sealed record CreateCurrencyCommand(string Code, string Name, decimal RateToBase);

public sealed class CreateCurrencyCommandHandler(IDbContextFactory<AppDbContext> dbContextFactory)
{
    public async Task<OperationResult<CurrencyResponse>> HandleAsync(
        CreateCurrencyCommand command,
        CancellationToken cancellationToken)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        var normalizedCode = command.Code.Trim().ToUpperInvariant();

        var codeExists = await dbContext.Currencies
            .AnyAsync(currency => currency.Code == normalizedCode, cancellationToken);

        if (codeExists)
        {
            return OperationResult<CurrencyResponse>.Conflict("Ya existe una moneda con ese código.");
        }

        var currency = new Currency
        {
            Code = normalizedCode,
            Name = command.Name.Trim(),
            RateToBase = command.RateToBase
        };

        dbContext.Currencies.Add(currency);
        await dbContext.SaveChangesAsync(cancellationToken);

        return OperationResult<CurrencyResponse>.Success(
            new CurrencyResponse(currency.Id, currency.Code, currency.Name, currency.RateToBase));
    }
}
