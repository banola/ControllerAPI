# ControllerAPI

API ASP.NET Core (.NET 10, controllers) que simula controles de jogo enviando teclas na máquina host e retorna prints da tela do jogo em base64.

## Restrições
- **Só executa em Windows** (Win32 `SendInput` + GDI). Compila em Linux/macOS (`EnableWindowsTargeting`), mas o `Program.cs` aborta fora do Windows.
- Rodar na sessão do usuário logado (não como Windows Service/sessão 0). Se o jogo roda como admin, a API também precisa (UIPI).
- Teclas são enviadas por **scan code** (funciona com DirectInput/RawInput, ao contrário de `SendKeys` do WinForms).
- Fullscreen exclusivo pode gerar print preto: usar modo janela/borderless.

## Estrutura (`src/ControllerApi`)
- `Controllers/ControlsController.cs` — `GET api/controls/keys`, `POST api/controls/{key}/press?durationMs=`, `/down`, `/up`
- `Controllers/ScreenController.cs` — `GET api/screen/screenshot?format=png|jpeg`, `GET api/status`
- `Services/Win32InputService.cs` — SendInput; `PressAsync` sempre solta a tecla (finally)
- `Services/GdiScreenCaptureService.cs` — captura área cliente da janela do jogo, ou a tela principal se não configurado
- `Services/GameWindowLocator.cs` — acha/foca janela por `Game:ProcessName` ou `Game:WindowTitle`
- `Models/GameKey.cs` — enum + mapa VK/scan code. **Nova tecla = adicionar no enum e no `GameKeyMap`**
- `Infrastructure/` — middleware `X-Api-Key` e mapeamento de exceções → ProblemDetails
- `Native/Win32.cs` — P/Invoke

Teclas atuais: W, A, S, D, Space, Esc (alias `Escape`).

## Config (`appsettings.json`)
- `Urls`: padrão `http://localhost:5000`. Para expor na rede use `http://0.0.0.0:5000` **e** defina `Security:ApiKey`.
- `Game`: `ProcessName` (sem .exe), `WindowTitle`, `FocusBeforeInput`, `DefaultPressDurationMs`, `MaxPressDurationMs`, `JpegQuality`.

## Comandos
```
dotnet build ControllerApi.sln
dotnet run --project src/ControllerApi        # Windows
dotnet publish src/ControllerApi -c Release -r win-x64
```
Exemplos de requisição: `src/ControllerApi/ControllerApi.http`. OpenAPI em `/openapi/v1.json` (Development).

## Agent jogador (`.claude/`)
- `agents/game-player.md` joga via API local (rodar: `claude --agent game-player` ou delegar ao subagent `game-player`). Skills: `game-controls`, `game-screenshot` (scripts bash/curl; precisa de `curl` e `base64`, ex.: Git Bash no Windows).
- Hooks: `settings.json` SessionStart checa a API; `hooks/guard.sh` (PreToolUse do agent) bloqueia >5 ações sem screenshot e respeita o kill switch `touch .claude/STOP` (apague para retomar).
- Env opcional: `CONTROLLER_API_URL`, `CONTROLLER_API_KEY`.
