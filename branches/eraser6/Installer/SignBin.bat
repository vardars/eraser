goto :Sign

:: pass params to a subroutine
SET _params=%*
CALL :Sign "%_params%"
GOTO :eof

:Sign
@rem Core binaries
signtool sign /f "%~1\Authenticode.pfx" "%~1\bin\Release\Eraser.exe"
signtool sign /f "%~1\Authenticode.pfx" "%~1\bin\Release\Eraser.Manager.dll"
signtool sign /f "%~1\Authenticode.pfx" "%~1\bin\Release\Eraser.Util.dll"
signtool sign /f "%~1\Authenticode.pfx" "%~1\bin\Release\Plugins\Eraser.DefaultPlugins.dll"

@rem Internationalisations
signtool sign /f "%~1\Authenticode.pfx" "%~1\bin\Release\en\Eraser.Manager.resources.dll"
signtool sign /f "%~1\Authenticode.pfx" "%~1\bin\Release\en\Eraser.resources.dll"
signtool sign /f "%~1\Authenticode.pfx" "%~1\bin\Release\nl\Eraser.Manager.resources.dll"
signtool sign /f "%~1\Authenticode.pfx" "%~1\bin\Release\nl\Eraser.resources.dll"
signtool sign /f "%~1\Authenticode.pfx" "%~1\bin\Release\Plugins\en\Eraser.DefaultPlugins.resources.dll"
signtool sign /f "%~1\Authenticode.pfx" "%~1\bin\Release\Plugins\nl\Eraser.DefaultPlugins.resources.dll"

