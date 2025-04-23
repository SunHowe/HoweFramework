set WORKSPACE=..
set LUBAN_DLL=%WORKSPACE%\Tools\Luban\Luban.dll
set LUBAN_TEMPLATE_DIR=%WORKSPACE%\Tools\LubanClientTemplate
set CONF_ROOT=.

set OUTPUT_DATA_DIR_CLIENT=%WORKSPACE%\Client\Assets\GameMain\DataTable
set OUTPUT_CODE_DIR_CLIENT=%WORKSPACE%\Client\Assets\GameMain\Scripts\DataTable\AutoGen

dotnet %LUBAN_DLL% ^
    --customTemplateDir %LUBAN_TEMPLATE_DIR% ^
    -t client ^
    -c cs-bin ^
    -d bin ^
    --conf %CONF_ROOT%\luban.conf ^
    -x outputDataDir=%OUTPUT_DATA_DIR_CLIENT% ^
    -x outputCodeDir=%OUTPUT_CODE_DIR_CLIENT%

pause