using System.ComponentModel;
using ControllerApi.Services;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ControllerApi.Infrastructure;

/// <summary>Converte erros conhecidos em ProblemDetails.</summary>
public class GameExceptionHandler(IProblemDetailsService problemDetails) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        var (status, title) = exception switch
        {
            GameWindowNotFoundException => (StatusCodes.Status404NotFound, "Janela do jogo não encontrada"),
            Win32Exception => (StatusCodes.Status500InternalServerError, "Falha na chamada Win32"),
            OperationCanceledException => (499, "Requisição cancelada"),
            _ => (0, null)
        };
        if (title is null)
            return false;

        context.Response.StatusCode = status;
        return await problemDetails.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = context,
            Exception = exception,
            ProblemDetails = new ProblemDetails { Status = status, Title = title, Detail = exception.Message }
        });
    }
}
