namespace ControllerApi.Options;

public class GameOptions
{
    public const string Section = "Game";

    /// <summary>Nome do processo do jogo (sem .exe). Tem prioridade sobre WindowTitle.</summary>
    public string? ProcessName { get; set; }

    /// <summary>Título exato da janela do jogo.</summary>
    public string? WindowTitle { get; set; }

    /// <summary>Traz a janela do jogo para o foco antes de enviar teclas.</summary>
    public bool FocusBeforeInput { get; set; } = true;

    public int DefaultPressDurationMs { get; set; } = 100;

    public int MaxPressDurationMs { get; set; } = 10_000;

    /// <summary>Qualidade JPEG (1-100) quando format=jpeg.</summary>
    public int JpegQuality { get; set; } = 80;

    public bool IsTargetConfigured =>
        !string.IsNullOrWhiteSpace(ProcessName) || !string.IsNullOrWhiteSpace(WindowTitle);
}

public class SecurityOptions
{
    public const string Section = "Security";

    /// <summary>Se definido, toda requisição precisa do header X-Api-Key com este valor.</summary>
    public string? ApiKey { get; set; }
}
