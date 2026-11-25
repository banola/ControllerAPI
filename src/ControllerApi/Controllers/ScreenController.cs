using ControllerApi.Models;
using ControllerApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace ControllerApi.Controllers;

[ApiController]
[Route("api")]
public class ScreenController(IScreenCaptureService capture, GameWindowLocator locator) : ControllerBase
{
    /// <summary>Print da tela do jogo em base64. format: png (padrão) | jpeg.</summary>
    [HttpGet("screen/screenshot")]
    public ActionResult<ScreenshotResponse> Screenshot([FromQuery] string format = "png")
    {
        if (!Enum.TryParse<ImageFormatOption>(format, ignoreCase: true, out var f) || !Enum.IsDefined(f))
            return Problem("format deve ser 'png' ou 'jpeg'.", statusCode: 400);
        return capture.Capture(f);
    }

    /// <summary>Indica se a janela do jogo configurada foi encontrada.</summary>
    [HttpGet("status")]
    public StatusResponse Status()
    {
        if (!locator.IsTargetConfigured)
            return new StatusResponse(false, false, null);
        try
        {
            var hwnd = locator.Find();
            return new StatusResponse(true, hwnd != IntPtr.Zero, null);
        }
        catch (GameWindowNotFoundException ex)
        {
            return new StatusResponse(true, false, ex.Message);
        }
    }
}
