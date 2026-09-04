#!/bin/bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
FGUI_BIN="$SCRIPT_DIR/bin/fgui.mjs"

if ! command -v node >/dev/null 2>&1; then
  echo "node is required (Node.js >= 20)." >&2
  exit 1
fi

if [ ! -d "$SCRIPT_DIR/node_modules" ]; then
  echo "Dependencies missing. Run: (cd \"$SCRIPT_DIR\" && npm install)" >&2
  exit 1
fi

exec node "$FGUI_BIN" publish "$@"
