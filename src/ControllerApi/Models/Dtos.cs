namespace ControllerApi.Models;

public record KeyActionResponse(string Key, string Action, int? DurationMs, DateTimeOffset ExecutedAt);

public record ScreenshotResponse(
    string Image,
    string Format,
    int Width,
    int Height,
    string Source,
    DateTimeOffset CapturedAt);

public record StatusResponse(bool TargetConfigured, bool WindowFound, string? WindowTitle);
