using ControllerApi.Models;

namespace ControllerApi.Services;

public interface IInputService
{
    /// <summary>Pressiona e solta a tecla após <paramref name="durationMs"/>.</summary>
    Task PressAsync(GameKey key, int durationMs, CancellationToken ct);

    /// <summary>Mantém a tecla pressionada até <see cref="Up"/> ser chamado.</summary>
    void Down(GameKey key);

    void Up(GameKey key);
}
