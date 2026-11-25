using ControllerApi.Models;

namespace ControllerApi.Services;

public enum ImageFormatOption
{
    Png,
    Jpeg
}

public interface IScreenCaptureService
{
    ScreenshotResponse Capture(ImageFormatOption format);
}
