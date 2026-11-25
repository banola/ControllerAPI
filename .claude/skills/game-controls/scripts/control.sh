#!/usr/bin/env bash
# Uso: control.sh press|down|up <W|A|S|D|Space|Esc> [durationMs]
set -euo pipefail
action="${1:-}"; key="${2:-}"; dur="${3:-}"
base="${CONTROLLER_API_URL:-http://localhost:5000}"

case "$action" in press|down|up) ;; *) echo "Uso: control.sh press|down|up <tecla> [durationMs]" >&2; exit 64;; esac
case "${key,,}" in w|a|s|d|space|esc) ;; *) echo "Tecla invalida: '$key' (W A S D Space Esc)" >&2; exit 64;; esac

url="$base/api/controls/$key/$action"
[ -n "$dur" ] && [ "$action" = press ] && url="$url?durationMs=$dur"

args=(-sS --fail-with-body -X POST -m 15)
[ -n "${CONTROLLER_API_KEY:-}" ] && args+=(-H "X-Api-Key: $CONTROLLER_API_KEY")
curl "${args[@]}" "$url"; echo
