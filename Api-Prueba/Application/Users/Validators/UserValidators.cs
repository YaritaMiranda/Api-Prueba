using FluentValidation;
using UserCurrencyApi.Contracts.Users;

namespace UserCurrencyApi.Application.Users.Validators;

public sealed class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty()
            .MaximumLength(120);

        RuleFor(request => request.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);
    }
}

public sealed class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty()
            .MaximumLength(120);

        RuleFor(request => request.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);
    }
}

public sealed class BulkCreateUsersRequestValidator : AbstractValidator<BulkCreateUsersRequest>
{
    public BulkCreateUsersRequestValidator()
    {
        RuleFor(request => request.Users)
            .NotNull()
            .NotEmpty();
    }
}
