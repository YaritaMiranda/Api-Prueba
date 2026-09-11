namespace UserCurrencyApi.Contracts.Currencies;

public sealed record CreateCurrencyRequest(string Code, string Name, decimal RateToBase);

public sealed record CurrencyResponse(int Id, string Code, string Name, decimal RateToBase);

public sealed record ConvertCurrencyRequest(string FromCurrencyCode, string ToCurrencyCode, decimal Amount);

public sealed record CurrencyConversionResponse(
    string FromCurrency,
    string ToCurrency,
    decimal OriginalAmount,
    decimal ConvertedAmount);
