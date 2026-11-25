using ControllerApi.Models;
using ControllerApi.Options;
using ControllerApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ControllerApi.Controllers;

[ApiController]
[Route("api/controls")]
public class ControlsController(IInputService input, IOptionsMonitor<GameOptions> options) : ControllerBase
{
    /// <summary>Lista as teclas suportadas.</summary>
    [HttpGet("keys")]
    public IEnumerable<string> GetKeys() => Enum.GetNames<GameKey>();

    /// <summary>Pressiona e solta a tecla (tap). durationMs = tempo segurando.</summary>
    [HttpPost("{key}/press")]
    public async Task<ActionResult<KeyActionResponse>> Press(string key, [FromQuery] int? durationMs, CancellationToken ct)
    {
        if (!GameKeyMap.TryParse(key, out var k))
            return UnknownKey(key);

        var o = options.CurrentValue;
        var duration = durationMs ?? o.DefaultPressDurationMs;
        if (duration < 1 || duration > o.MaxPressDurationMs)
            return Problem($"durationMs deve estar entre 1 e {o.MaxPressDurationMs}.", statusCode: 400);

        await input.PressAsync(k, duration, ct);
        return new KeyActionResponse(k.ToString(), "press", duration, DateTimeOffset.UtcNow);
    }

    /// <summary>Mantém a tecla pressionada (ex.: andar continuamente). Libere com /up.</summary>
    [HttpPost("{key}/down")]
    public ActionResult<KeyActionResponse> Down(string key)
    {
        if (!GameKeyMap.TryParse(key, out var k))
            return UnknownKey(key);
        input.Down(k);
        return new KeyActionResponse(k.ToString(), "down", null, DateTimeOffset.UtcNow);
    }

    /// <summary>Solta uma tecla mantida com /down.</summary>
    [HttpPost("{key}/up")]
    public ActionResult<KeyActionResponse> Up(string key)
    {
        if (!GameKeyMap.TryParse(key, out var k))
            return UnknownKey(key);
        input.Up(k);
        return new KeyActionResponse(k.ToString(), "up", null, DateTimeOffset.UtcNow);
    }

    private ObjectResult UnknownKey(string key) =>
        Problem($"Tecla '{key}' não suportada. Válidas: {string.Join(", ", Enum.GetNames<GameKey>())}.", statusCode: 400);
}
