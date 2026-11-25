using System.Drawing;
using System.Drawing.Imaging;
using ControllerApi.Models;
using ControllerApi.Native;
using ControllerApi.Options;
using Microsoft.Extensions.Options;

namespace ControllerApi.Services;

/// <summary>
/// Captura via GDI (CopyFromScreen). Captura a área cliente do jogo se configurado,
/// senão a tela principal. Jogos em fullscreen exclusivo podem retornar imagem preta — use modo janela/borderless.
/// </summary>
public class GdiScreenCaptureService(GameWindowLocator locator, IOptionsMonitor<GameOptions> options) : IScreenCaptureService
{
    public ScreenshotResponse Capture(ImageFormatOption format)
    {
        var hwnd = locator.Find();
        var area = hwnd != IntPtr.Zero
            ? locator.GetClientArea(hwnd)
            : new Rectangle(0, 0, Win32.GetSystemMetrics(Win32.SM_CXSCREEN), Win32.GetSystemMetrics(Win32.SM_CYSCREEN));

        if (area.Width <= 0 || area.Height <= 0)
            throw new GameWindowNotFoundException("Janela do jogo está minimizada ou sem área visível.");

        using var bitmap = new Bitmap(area.Width, area.Height, PixelFormat.Format32bppArgb);
        using (var g = Graphics.FromImage(bitmap))
            g.CopyFromScreen(area.Location, Point.Empty, area.Size, CopyPixelOperation.SourceCopy);

        using var ms = new MemoryStream();
        if (format == ImageFormatOption.Jpeg)
        {
            var codec = ImageCodecInfo.GetImageEncoders().First(c => c.FormatID == ImageFormat.Jpeg.Guid);
            using var p = new EncoderParameters(1);
            p.Param[0] = new EncoderParameter(Encoder.Quality, (long)Math.Clamp(options.CurrentValue.JpegQuality, 1, 100));
            bitmap.Save(ms, codec, p);
        }
        else
        {
            bitmap.Save(ms, ImageFormat.Png);
        }

        return new ScreenshotResponse(
            Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length),
            format == ImageFormatOption.Jpeg ? "image/jpeg" : "image/png",
            area.Width,
            area.Height,
            hwnd != IntPtr.Zero ? "window" : "screen",
            DateTimeOffset.UtcNow);
    }
}
