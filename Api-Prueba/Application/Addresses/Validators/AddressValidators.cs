using FluentValidation;
using UserCurrencyApi.Contracts.Addresses;

namespace UserCurrencyApi.Application.Addresses.Validators;

public sealed class CreateAddressRequestValidator : AbstractValidator<CreateAddressRequest>
{
    public CreateAddressRequestValidator()
    {
        RuleFor(request => request.Street)
            .NotEmpty()
            .MaximumLength(180);

        RuleFor(request => request.City)
            .NotEmpty()
            .MaximumLength(120);

        RuleFor(request => request.Country)
            .NotEmpty()
            .MaximumLength(120);

        RuleFor(request => request.ZipCode)
            .MaximumLength(30);
    }
}

public sealed class UpdateAddressRequestValidator : AbstractValidator<UpdateAddressRequest>
{
    public UpdateAddressRequestValidator()
    {
        RuleFor(request => request.Street)
            .NotEmpty()
            .MaximumLength(180);

        RuleFor(request => request.City)
            .NotEmpty()
            .MaximumLength(120);

        RuleFor(request => request.Country)
            .NotEmpty()
            .MaximumLength(120);

        RuleFor(request => request.ZipCode)
            .MaximumLength(30);
    }
}
