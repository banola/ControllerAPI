#!/usr/bin/env bash
# SessionStart: informa ao Claude se a ControllerAPI esta no ar.
base="${CONTROLLER_API_URL:-http://localhost:5000}"
args=(-sS -m 3); [ -n "${CONTROLLER_API_KEY:-}" ] && args+=(-H "X-Api-Key: $CONTROLLER_API_KEY")
if out=$(curl "${args[@]}" "$base/api/status" 2>&1); then
  echo "ControllerAPI online em $base: $out"
else
  echo "ControllerAPI OFFLINE em $base. Peca ao usuario para rodar: dotnet run --project src/ControllerApi (Windows)."
fi
