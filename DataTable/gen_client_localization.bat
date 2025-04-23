set WORKSPACE=..
set LUBAN_DLL=%WORKSPACE%\Tools\Luban\Luban.dll
set LUBAN_TEMPLATE_DIR=%WORKSPACE%\Tools\LubanClientTemplate
set CONF_ROOT=.

@REM set OUTPUT_DATA_DIR_LAUNCHER=%WORKSPACE%\Client\Assets\GameLauncher\Localization
set OUTPUT_DATA_DIR_MAIN=%WORKSPACE%\Client\Assets\GameMain\Localization

@REM dotnet %LUBAN_DLL% ^
@REM     -t client ^
@REM     -d bin ^
@REM     --conf %CONF_ROOT%\luban_localization_launch.conf ^
@REM     -x outputDataDir=%OUTPUT_DATA_DIR_LAUNCHER%

dotnet %LUBAN_DLL% ^
    -t client ^
    -d bin ^
    --conf %CONF_ROOT%\luban_localization.conf ^
    -x outputDataDir=%OUTPUT_DATA_DIR_MAIN%

pause