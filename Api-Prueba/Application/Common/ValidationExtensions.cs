using FluentValidation;
using FluentValidation.Results;

namespace UserCurrencyApi.Application.Common;

public static class ValidationExtensions
{
    public static async Task<IResult?> ValidateOrProblemAsync<T>(
        this IValidator<T> validator,
        T request,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        return validationResult.IsValid
            ? null
            : Results.ValidationProblem(validationResult.ToErrorDictionary());
    }

    public static Dictionary<string, string[]> ToErrorDictionary(this ValidationResult validationResult)
    {
        return validationResult.Errors
            .GroupBy(error => error.PropertyName)
            .ToDictionary(
                group => group.Key,
                group => group.Select(error => error.ErrorMessage).ToArray());
    }
}
