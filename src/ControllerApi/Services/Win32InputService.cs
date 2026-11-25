using System.ComponentModel;
using System.Runtime.InteropServices;
using ControllerApi.Models;
using ControllerApi.Native;
using ControllerApi.Options;
using Microsoft.Extensions.Options;

namespace ControllerApi.Services;

/// <summary>
/// "SendKeys" via Win32 SendInput com scan codes — funciona em jogos que ignoram
/// System.Windows.Forms.SendKeys (DirectInput/RawInput).
/// </summary>
public class Win32InputService(GameWindowLocator locator, IOptionsMonitor<GameOptions> options) : IInputService
{
    private static readonly int InputSize = Marshal.SizeOf<Win32.INPUT>();
    private readonly Lock _sync = new();

    public async Task PressAsync(GameKey key, int durationMs, CancellationToken ct)
    {
        Down(key);
        try
        {
            await Task.Delay(durationMs, ct);
        }
        finally
        {
            Up(key); // garante que a tecla nunca fica "presa"
        }
    }

    public void Down(GameKey key) => Send(key, keyUp: false);

    public void Up(GameKey key) => Send(key, keyUp: true);

    private void Send(GameKey key, bool keyUp)
    {
        lock (_sync)
        {
            if (options.CurrentValue.FocusBeforeInput)
            {
                try
                {
                    locator.Focus(locator.Find());
                }
                catch (GameWindowNotFoundException) when (keyUp)
                {
                    // Jogo fechou com a tecla pressionada: solta mesmo assim.
                }
            }

            var (vk, scan) = GameKeyMap.Get(key);
            var input = new Win32.INPUT
            {
                type = Win32.INPUT_KEYBOARD,
                U = new Win32.InputUnion
                {
                    ki = new Win32.KEYBDINPUT
                    {
                        wVk = vk,
                        wScan = scan,
                        dwFlags = Win32.KEYEVENTF_SCANCODE | (keyUp ? Win32.KEYEVENTF_KEYUP : 0)
                    }
                }
            };

            if (Win32.SendInput(1, [input], InputSize) != 1)
                throw new Win32Exception(Marshal.GetLastWin32Error(), "SendInput falhou (UIPI/sessão sem desktop?).");
        }
    }
}
