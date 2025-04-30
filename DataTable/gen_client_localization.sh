#!/bin/bash

WORKSPACE=..
LUBAN_DLL="$WORKSPACE/Tools/Luban/Luban.dll"
LUBAN_TEMPLATE_DIR="$WORKSPACE/Tools/LubanClientTemplate"
CONF_ROOT=.

# OUTPUT_DATA_DIR_LAUNCHER="$WORKSPACE/Client/Assets/GameLauncher/Localization"
OUTPUT_DATA_DIR_MAIN="$WORKSPACE/Client/Assets/GameMain/Localization"

# dotnet "$LUBAN_DLL" \
#     -t client \
#     -d bin \
#     --conf "$CONF_ROOT/luban_localization_launch.conf" \
#     -x outputDataDir="$OUTPUT_DATA_DIR_LAUNCHER"

dotnet "$LUBAN_DLL" \
    -t client \
    -d bin \
    --conf "$CONF_ROOT/luban_localization.conf" \
    -x outputDataDir="$OUTPUT_DATA_DIR_MAIN"

# read -p "Press any key to continue..."