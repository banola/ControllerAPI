---
name: game-screenshot
description: Captura a tela do jogo via ControllerAPI e a salva como imagem para analise. Use para entender o estado do jogo antes e depois de agir.
---

# game-screenshot

```
bash .claude/skills/game-screenshot/scripts/screenshot.sh [jpeg|png]
```

Imprime o caminho da imagem (`.claude/game-state/latest.jpg`). **Abra com a ferramenta Read** para ver a tela. Use `png` so se precisar ler texto pequeno.

Ao analisar: onde esta o personagem, objetivo visivel, perigos, menus/dialogos, HUD (vida, itens), e se a ultima acao teve efeito. Se a imagem for preta, o jogo provavelmente esta em tela cheia exclusiva: peca ao usuario modo janela/borderless.
