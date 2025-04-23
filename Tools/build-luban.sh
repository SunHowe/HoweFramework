#!/bin/bash

[ -d Luban ] && rm -rf Luban

dotnet build  LubanProject/src/Luban/Luban.csproj -c Release -o Luban