cd %1
if "%3" == "" (
	@echo "Usage: gettext <Folder to search for files> <Output folder> <Template name>"
	exit 1
)

if exist "Filelist" (
	echo The temporary file holding translation files already exist.
	exit 1
)

if not exist "Languages" (
	mkdir "Languages"
)

for /R %%i in (*.cs) do echo %%i >> "Filelist"
xgettext --keyword=_ --keyword=_:1c,2 --from-code UTF-8 -o "Languages\%3.pot" -f "Filelist"
del Filelist

for /R %%i in (*.designer.cs) do echo %%i >> "Filelist"
xgettext --keyword=_ --keyword=_:1c,2 --from-code UTF-8 -a -j -o "Languages\%3.pot" -f Filelist
del Filelist
for %%i in (Languages\*.po) do msgmerge -U --backup=none "%%i" "Languages\%3.pot"

set path=%PATH%;C:\Windows\Microsoft.NET\Framework\v2.0.50727
set GETTEXTCSHARPLIBDIR=D:\Development\Libraries\C#\gettext\bin\Release
echo %PATH%
for %%i in (Languages\*.po) do msgfmt --verbose --csharp %%i --locale %%~ni  -r Eraser -d %2
