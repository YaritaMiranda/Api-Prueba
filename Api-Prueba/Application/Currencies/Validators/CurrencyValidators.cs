using FluentValidation;
using UserCurrencyApi.Contracts.Currencies;

namespace UserCurrencyApi.Application.Currencies.Validators;

public sealed class CreateCurrencyRequestValidator : AbstractValidator<CreateCurrencyRequest>
{
    public CreateCurrencyRequestValidator()
    {
        RuleFor(request => request.Code)
            .NotEmpty()
            .Length(3);

        RuleFor(request => request.Name)
            .NotEmpty()
            .MaximumLength(120);

        RuleFor(request => request.RateToBase)
            .GreaterThan(0);
    }
}

public sealed class ConvertCurrencyRequestValidator : AbstractValidator<ConvertCurrencyRequest>
{
    public ConvertCurrencyRequestValidator()
    {
        RuleFor(request => request.Amount)
            .GreaterThan(0);

        RuleFor(request => request.FromCurrencyCode)
            .NotEmpty()
            .Length(3);

        RuleFor(request => request.ToCurrencyCode)
            .NotEmpty()
            .Length(3);
    }
}
