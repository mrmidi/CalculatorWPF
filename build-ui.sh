#!/usr/bin/env bash
set -euo pipefail
UI=${UI:-avalonia}
CMD=${1:-run}

case "$UI" in
  avalonia)
    PROJECT="Calculator.Avalonia/Calculator.Avalonia.csproj"
    FRAMEWORK="net10.0"
    ;;
  wpf)
    PROJECT="CalculatorWPF/CalculatorWPF.csproj"
    FRAMEWORK="net10.0-windows"
    ;;
  *)
    echo "Unknown UI: $UI" >&2
    exit 1
    ;;
esac

if [ "$CMD" = "run" ]; then
  dotnet run --project "$PROJECT" -f "$FRAMEWORK"
else
  dotnet "$CMD" --project "$PROJECT"
fi
