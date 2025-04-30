#!/bin/bash

WORKSPACE=..
LUBAN_DLL="$WORKSPACE/Tools/Luban/Luban.dll"
LUBAN_TEMPLATE_DIR="$WORKSPACE/Tools/LubanServerTemplate"
CONF_ROOT=.

OUTPUT_DATA_DIR_SERVER="$WORKSPACE/Server/bin/DataTable"
OUTPUT_CODE_DIR_SERVER="$WORKSPACE/Server/DataTable/AutoGen"

dotnet "$LUBAN_DLL" \
    --customTemplateDir "$LUBAN_TEMPLATE_DIR" \
    -t server \
    -c cs-newtonsoft-json \
    -d json \
    --conf "$CONF_ROOT/luban.conf" \
    -x outputDataDir="$OUTPUT_DATA_DIR_SERVER" \
    -x outputCodeDir="$OUTPUT_CODE_DIR_SERVER"

# 暂停功能在Unix shell中通常不需要
# 如果需要暂停，可以使用read命令
# read -p "Press enter to continue"