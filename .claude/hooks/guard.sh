#!/usr/bin/env bash
# PreToolUse(Bash) do agente game-player.
#  1) Kill switch: se existir .claude/STOP, bloqueia tudo.
#  2) Anti-cegueira: bloqueia > MAX acoes seguidas sem screenshot.
input=$(cat)
root="${CLAUDE_PROJECT_DIR:-.}/.claude"
max="${GAME_MAX_ACTIONS_WITHOUT_SCREENSHOT:-5}"
counter="$root/game-state/actions.count"; mkdir -p "$root/game-state"

if [ -f "$root/STOP" ]; then
  echo "Kill switch ativo (.claude/STOP). Pare de jogar e avise o usuario." >&2; exit 2
fi
if printf '%s' "$input" | grep -q 'screenshot\.sh'; then echo 0 > "$counter"; exit 0; fi
if printf '%s' "$input" | grep -q 'control\.sh'; then
  n=$(( $(cat "$counter" 2>/dev/null || echo 0) + 1 ))
  if [ "$n" -gt "$max" ]; then
    echo "Ja foram $max acoes sem olhar a tela. Tire um screenshot antes de continuar." >&2; exit 2
  fi
  echo "$n" > "$counter"
fi
exit 0
