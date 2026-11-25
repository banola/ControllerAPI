using System.Security.Cryptography;
using System.Text;
using ControllerApi.Options;
using Microsoft.Extensions.Options;

namespace ControllerApi.Infrastructure;

/// <summary>Exige header X-Api-Key quando Security:ApiKey está configurado.</summary>
public class ApiKeyMiddleware(RequestDelegate next, IOptionsMonitor<SecurityOptions> options)
{
    public const string Header = "X-Api-Key";

    public Task InvokeAsync(HttpContext context)
    {
        var expected = options.CurrentValue.ApiKey;
        if (string.IsNullOrEmpty(expected))
            return next(context);

        var provided = context.Request.Headers[Header].ToString();
        if (CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(provided), Encoding.UTF8.GetBytes(expected)))
            return next(context);

        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    }
}
