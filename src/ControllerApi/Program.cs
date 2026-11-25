using ControllerApi.Infrastructure;
using ControllerApi.Native;
using ControllerApi.Options;
using ControllerApi.Services;

if (!OperatingSystem.IsWindows())
    throw new PlatformNotSupportedException("ControllerApi precisa rodar em Windows (SendInput/GDI).");

// Coordenadas em pixels físicos (evita print cortado com escala de tela != 100%).
Win32.SetProcessDpiAwarenessContext(Win32.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2);

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<GameOptions>(builder.Configuration.GetSection(GameOptions.Section));
builder.Services.Configure<SecurityOptions>(builder.Configuration.GetSection(SecurityOptions.Section));

builder.Services.AddSingleton<GameWindowLocator>();
builder.Services.AddSingleton<IInputService, Win32InputService>();
builder.Services.AddSingleton<IScreenCaptureService, GdiScreenCaptureService>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GameExceptionHandler>();

var app = builder.Build();

app.UseExceptionHandler();
app.UseMiddleware<ApiKeyMiddleware>();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.MapControllers();

app.Run();
