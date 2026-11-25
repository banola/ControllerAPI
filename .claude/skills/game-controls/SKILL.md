---
name: game-controls
description: Envia teclas ao jogo via ControllerAPI (W A S D Space Esc). Use para mover, pular/agir ou pausar o jogo.
---

# game-controls

```
bash .claude/skills/game-controls/scripts/control.sh press <tecla> [durationMs]   # toque (padrao 100ms)
bash .claude/skills/game-controls/scripts/control.sh down  <tecla>                # segura
bash .claude/skills/game-controls/scripts/control.sh up    <tecla>                # solta
```

Teclas: `W A S D Space Esc`. Env: `CONTROLLER_API_URL` (padrao `http://localhost:5000`), `CONTROLLER_API_KEY`.

- Passos curtos: `press W 150-300`. Movimento continuo: `down`, observe, depois **sempre** `up`.
- Todo `down` precisa de um `up` correspondente antes de terminar ou perguntar ao usuario.
- `Esc` normalmente abre/fecha menu de pausa: use so com motivo (menu, travado em dialogo).
- Erro 404 = janela do jogo fechada; erro de conexao = API offline. Nao insista: avise o usuario.
