set WORKSPACE=..
set LUBAN_DLL=%WORKSPACE%\Tools\Luban\Luban.dll
set LUBAN_TEMPLATE_DIR=%WORKSPACE%\Tools\LubanServerTemplate
set CONF_ROOT=.

set OUTPUT_DATA_DIR_CLIENT=%WORKSPACE%\GeekServer\bin\app_debug\Bytes
set OUTPUT_CODE_DIR_CLIENT=%WORKSPACE%\GeekServer\Geek.Server.Generate\DataTable\Generate

dotnet %LUBAN_DLL% ^
    --customTemplateDir %LUBAN_TEMPLATE_DIR% ^
    -t server ^
    -c cs-bin ^
    -d bin ^
    --conf %CONF_ROOT%\luban.conf ^
    -x outputDataDir=%OUTPUT_DATA_DIR_CLIENT% ^
    -x outputCodeDir=%OUTPUT_CODE_DIR_CLIENT%

pause