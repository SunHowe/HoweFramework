set WORKSPACE=..
set LUBAN_DLL=%WORKSPACE%\Tools\Luban\Luban.dll
set LUBAN_TEMPLATE_DIR=%WORKSPACE%\Tools\LubanServerTemplate
set CONF_ROOT=.

set OUTPUT_DATA_DIR_SERVER=%WORKSPACE%\Server\bin\DataTable
set OUTPUT_CODE_DIR_SERVER=%WORKSPACE%\Server\DataTable\AutoGen

dotnet %LUBAN_DLL% ^
    --customTemplateDir %LUBAN_TEMPLATE_DIR% ^
    -t server ^
    -c cs-newtonsoft-json ^
    -d json ^
    --conf %CONF_ROOT%\luban.conf ^
    -x outputDataDir=%OUTPUT_DATA_DIR_SERVER% ^
    -x outputCodeDir=%OUTPUT_CODE_DIR_SERVER%

pause