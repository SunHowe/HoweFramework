#!/bin/bash

WORKSPACE=..
LUBAN_DLL="$WORKSPACE/Tools/Luban/Luban.dll"
LUBAN_TEMPLATE_DIR="$WORKSPACE/Tools/LubanServerTemplate"
CONF_ROOT=.

OUTPUT_DATA_DIR_SERVER="$WORKSPACE/GeekServer/bin/app_debug/Bytes"
OUTPUT_CODE_DIR_SERVER="$WORKSPACE/GeekServer/Geek.Server.Generate/DataTable/Generate"

dotnet "$LUBAN_DLL" \
    --customTemplateDir "$LUBAN_TEMPLATE_DIR" \
    -t server \
    -c cs-bin \
    -d bin \
    --conf "$CONF_ROOT/luban.conf" \
    -x outputDataDir="$OUTPUT_DATA_DIR_SERVER" \
    -x outputCodeDir="$OUTPUT_CODE_DIR_SERVER"

# pause equivalent in shell
# read -p "Press any key to continue..."