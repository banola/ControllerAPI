using System.Diagnostics;
using System.Drawing;
using ControllerApi.Native;
using ControllerApi.Options;
using Microsoft.Extensions.Options;

namespace ControllerApi.Services;

public class GameWindowNotFoundException(string message) : Exception(message);

/// <summary>Localiza a janela do jogo configurada em <see cref="GameOptions"/>.</summary>
public class GameWindowLocator(IOptionsMonitor<GameOptions> options, ILogger<GameWindowLocator> logger)
{
    public bool IsTargetConfigured => options.CurrentValue.IsTargetConfigured;

    /// <summary>Retorna o handle da janela, <see cref="IntPtr.Zero"/> se não houver alvo configurado,
    /// ou lança <see cref="GameWindowNotFoundException"/> se o alvo configurado não estiver aberto.</summary>
    public IntPtr Find()
    {
        var o = options.CurrentValue;
        if (!o.IsTargetConfigured)
            return IntPtr.Zero;

        if (!string.IsNullOrWhiteSpace(o.ProcessName))
        {
            var name = Path.GetFileNameWithoutExtension(o.ProcessName);
            foreach (var p in Process.GetProcessesByName(name))
            {
                using (p)
                {
                    if (p.MainWindowHandle != IntPtr.Zero)
                        return p.MainWindowHandle;
                }
            }
            throw new GameWindowNotFoundException($"Processo '{name}' não encontrado ou sem janela.");
        }

        var hwnd = Win32.FindWindow(null, o.WindowTitle);
        return hwnd != IntPtr.Zero
            ? hwnd
            : throw new GameWindowNotFoundException($"Janela '{o.WindowTitle}' não encontrada.");
    }

    public void Focus(IntPtr hwnd)
    {
        if (hwnd == IntPtr.Zero || Win32.GetForegroundWindow() == hwnd)
            return;
        if (Win32.IsIconic(hwnd))
            Win32.ShowWindow(hwnd, Win32.SW_RESTORE);
        if (!Win32.SetForegroundWindow(hwnd))
            logger.LogWarning("SetForegroundWindow falhou; as teclas irão para a janela em foco.");
    }

    /// <summary>Área cliente da janela (sem bordas/título) em coordenadas de tela.</summary>
    public Rectangle GetClientArea(IntPtr hwnd)
    {
        if (!Win32.GetClientRect(hwnd, out var r))
            throw new GameWindowNotFoundException("Não foi possível obter as dimensões da janela do jogo.");
        var origin = new Win32.POINT();
        Win32.ClientToScreen(hwnd, ref origin);
        return new Rectangle(origin.X, origin.Y, r.Right - r.Left, r.Bottom - r.Top);
    }
}
