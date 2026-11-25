namespace ControllerApi.Models;

/// <summary>Teclas suportadas. Para adicionar uma nova: inclua aqui e mapeie em <see cref="GameKeyMap"/>.</summary>
public enum GameKey
{
    W,
    A,
    S,
    D,
    Space,
    Esc
}

public static class GameKeyMap
{
    // Virtual-key code + scan code (set 1). Jogos (DirectInput/RawInput) costumam ler o scan code.
    private static readonly Dictionary<GameKey, (ushort Vk, ushort Scan)> Map = new()
    {
        [GameKey.W] = (0x57, 0x11),
        [GameKey.A] = (0x41, 0x1E),
        [GameKey.S] = (0x53, 0x1F),
        [GameKey.D] = (0x44, 0x20),
        [GameKey.Space] = (0x20, 0x39),
        [GameKey.Esc] = (0x1B, 0x01),
    };

    public static (ushort Vk, ushort Scan) Get(GameKey key) => Map[key];

    /// <summary>Parse case-insensitive, aceita "Escape" como alias e rejeita valores numéricos.</summary>
    public static bool TryParse(string? value, out GameKey key)
    {
        key = default;
        if (string.IsNullOrWhiteSpace(value) || char.IsDigit(value[0]) || value[0] == '-')
            return false;
        if (value.Equals("escape", StringComparison.OrdinalIgnoreCase))
        {
            key = GameKey.Esc;
            return true;
        }
        return Enum.TryParse(value, ignoreCase: true, out key) && Enum.IsDefined(key);
    }
}
