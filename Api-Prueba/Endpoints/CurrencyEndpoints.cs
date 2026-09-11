using FluentValidation;
using UserCurrencyApi.Application.Common;
using UserCurrencyApi.Application.Currencies.Commands;
using UserCurrencyApi.Application.Currencies.Queries;
using UserCurrencyApi.Application.CurrencyConversion;
using UserCurrencyApi.Contracts.Currencies;

namespace UserCurrencyApi.Endpoints;

public static class CurrencyEndpoints
{
    public static RouteGroupBuilder MapCurrencyEndpoints(this IEndpointRouteBuilder app)
    {
        var currenciesGroup = app.MapGroup("/currencies")
            .WithTags("Currencies");

        currenciesGroup.MapGet("/", async (
            GetCurrenciesQueryHandler handler,
            CancellationToken cancellationToken) =>
        {
            var currencies = await handler.HandleAsync(cancellationToken);
            return Results.Ok(currencies);
        });

        currenciesGroup.MapPost("/", async (
            CreateCurrencyRequest request,
            IValidator<CreateCurrencyRequest> validator,
            CreateCurrencyCommandHandler handler,
            CancellationToken cancellationToken) =>
        {
            var validationProblem = await validator.ValidateOrProblemAsync(request, cancellationToken);
            if (validationProblem is not null)
            {
                return validationProblem;
            }

            var result = await handler.HandleAsync(
                new CreateCurrencyCommand(request.Code, request.Name, request.RateToBase),
                cancellationToken);

            return result.ToHttpResult(currency => Results.Created($"/currencies/{currency.Id}", currency));
        });

        app.MapPost("/currency/convert", async (
            ConvertCurrencyRequest request,
            IValidator<ConvertCurrencyRequest> validator,
            ConvertCurrencyCommandHandler handler,
            CancellationToken cancellationToken) =>
        {
            var validationProblem = await validator.ValidateOrProblemAsync(request, cancellationToken);
            if (validationProblem is not null)
            {
                return validationProblem;
            }

            var result = await handler.HandleAsync(
                new ConvertCurrencyCommand(request.FromCurrencyCode, request.ToCurrencyCode, request.Amount),
                cancellationToken);

            return result.ToHttpResult(Results.Ok);
        })
        .WithTags("Currency conversion");

        return currenciesGroup;
    }
}
