#!/usr/bin/env bash
# Uso: screenshot.sh [png|jpeg]  -> salva a imagem e imprime o caminho (leia com a ferramenta Read)
set -euo pipefail
fmt="${1:-jpeg}"
base="${CONTROLLER_API_URL:-http://localhost:5000}"
dir="${CLAUDE_PROJECT_DIR:-.}/.claude/game-state"; mkdir -p "$dir"
ext=$([ "$fmt" = png ] && echo png || echo jpg)

args=(-sS --fail-with-body -m 20)
[ -n "${CONTROLLER_API_KEY:-}" ] && args+=(-H "X-Api-Key: $CONTROLLER_API_KEY")
json=$(curl "${args[@]}" "$base/api/screen/screenshot?format=$fmt")

b64=$(printf '%s' "$json" | grep -o '"image":"[^"]*"' | head -1 | sed 's/^"image":"//; s/"$//')
[ -n "$b64" ] || { echo "Resposta sem imagem: ${json:0:200}" >&2; exit 1; }
out="$dir/latest.$ext"
printf '%s' "$b64" | base64 -d > "$out"
echo "$out"
