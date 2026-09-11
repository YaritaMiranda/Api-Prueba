using UserCurrencyApi.Contracts.Common;

namespace UserCurrencyApi.Application.Common;

public static class HttpResultExtensions
{
    public static IResult ToHttpResult<T>(this OperationResult<T> result, Func<T, IResult> onSuccess)
    {
        return result.Status switch
        {
            OperationStatus.Success when result.Value is not null => onSuccess(result.Value),
            OperationStatus.NotFound => Results.NotFound(new ErrorResponse(result.Error ?? "Recurso no encontrado.")),
            OperationStatus.Conflict => Results.Conflict(new ErrorResponse(result.Error ?? "Conflicto al procesar la solicitud.")),
            _ => Results.Problem("No se pudo procesar la solicitud.")
        };
    }
}
