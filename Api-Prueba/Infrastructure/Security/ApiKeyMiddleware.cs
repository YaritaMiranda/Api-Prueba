using System.Security.Cryptography;
using System.Text;
using UserCurrencyApi.Contracts.Common;

namespace UserCurrencyApi.Infrastructure.Security;

public sealed class ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration)
{
    public const string HeaderName = "X-API-KEY";

    public async Task InvokeAsync(HttpContext context)
    {
        var configuredApiKey = configuration["ApiKey"];
        var hasApiKey = context.Request.Headers.TryGetValue(HeaderName, out var providedApiKey);

        if (string.IsNullOrWhiteSpace(configuredApiKey) ||
            !hasApiKey ||
            !IsValidApiKey(providedApiKey.ToString(), configuredApiKey))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(
                new ErrorResponse("API key ausente o incorrecta."),
                cancellationToken: context.RequestAborted);
            return;
        }

        await next(context);
    }

    private static bool IsValidApiKey(string providedApiKey, string configuredApiKey)
    {
        var providedBytes = Encoding.UTF8.GetBytes(providedApiKey);
        var configuredBytes = Encoding.UTF8.GetBytes(configuredApiKey);

        return providedBytes.Length == configuredBytes.Length &&
            CryptographicOperations.FixedTimeEquals(providedBytes, configuredBytes);
    }
}
