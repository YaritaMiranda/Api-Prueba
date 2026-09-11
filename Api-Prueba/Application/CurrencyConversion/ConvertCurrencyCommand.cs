using Microsoft.EntityFrameworkCore;
using UserCurrencyApi.Application.Common;
using UserCurrencyApi.Contracts.Currencies;
using UserCurrencyApi.Domain.Entities;
using UserCurrencyApi.Infrastructure.Data;

namespace UserCurrencyApi.Application.CurrencyConversion;

public sealed record ConvertCurrencyCommand(string FromCurrencyCode, string ToCurrencyCode, decimal Amount);

public sealed class ConvertCurrencyCommandHandler(IDbContextFactory<AppDbContext> dbContextFactory)
{
    public async Task<OperationResult<CurrencyConversionResponse>> HandleAsync(
        ConvertCurrencyCommand command,
        CancellationToken cancellationToken)
    {
        var fromCurrencyTask = GetCurrencyAsync(command.FromCurrencyCode, cancellationToken);
        var toCurrencyTask = GetCurrencyAsync(command.ToCurrencyCode, cancellationToken);

        await Task.WhenAll(fromCurrencyTask, toCurrencyTask);

        var fromCurrency = await fromCurrencyTask;
        var toCurrency = await toCurrencyTask;

        if (fromCurrency is null)
        {
            return OperationResult<CurrencyConversionResponse>.NotFound("Moneda origen no encontrada.");
        }

        if (toCurrency is null)
        {
            return OperationResult<CurrencyConversionResponse>.NotFound("Moneda destino no encontrada.");
        }

        var amountInBase = command.Amount * fromCurrency.RateToBase;
        var convertedAmount = amountInBase / toCurrency.RateToBase;

        return OperationResult<CurrencyConversionResponse>.Success(
            new CurrencyConversionResponse(
                fromCurrency.Code,
                toCurrency.Code,
                command.Amount,
                decimal.Round(convertedAmount, 6)));
    }

    private async Task<Currency?> GetCurrencyAsync(string code, CancellationToken cancellationToken)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        var normalizedCode = code.Trim().ToUpperInvariant();

        return await dbContext.Currencies
            .AsNoTracking()
            .SingleOrDefaultAsync(currency => currency.Code == normalizedCode, cancellationToken);
    }
}
