rem @echo off
IF NOT "%~1"=="" GOTO lbl_1_end
type _internal\build-bat-help
EXIT /B 1
:lbl_1_end
IF "%~3"=="" GOTO lbl_2_end
echo Too many parameters specified
echo Did you enclose parameters to be passed to the compiler in double quotes?
EXIT /B 1
:lbl_2_end
CHDIR out
IF EXIST * DEL /F /Q *
CHDIR ..\_tmp
IF EXIST * DEL /F /Q *
CHDIR ..
COPY src\* _tmp
IF "%~1"=="csc" GOTO lbl_3_csc
IF "%~1"=="mono" GOTO lbl_3_mono
GOTO lbl_3___error
:lbl_3_csc
CHDIR _tmp
csc /nowarn:0162,0169,0219 /nologo /target:library /out:alglib.dll %~2 *.cs >> ../log.txt 2>&1
IF NOT ERRORLEVEL 1 GOTO lbl_4
echo Error while compiling (see ../log.txt for more info)
CHDIR ..
EXIT /B 1
:lbl_4
CHDIR ..
COPY _tmp\alglib.* out > NUL 2> NUL
CHDIR _tmp
IF EXIST * DEL /F /Q *
CHDIR ..
GOTO lbl_3___end
:lbl_3_mono
CHDIR _tmp
CALL mcs /nowarn:0162,0169,0219 /target:library /out:libalglib.dll %~2 *.cs >> ../log.txt 2>&1
IF NOT ERRORLEVEL 1 GOTO lbl_5
echo Error while compiling (see ../log.txt for more info)
CHDIR ..
EXIT /B 1
:lbl_5
CHDIR ..
COPY _tmp\libalglib.dll out > NUL 2> NUL
CHDIR _tmp
IF EXIST * DEL /F /Q *
CHDIR ..
GOTO lbl_3___end
:lbl_3___error
echo unknown compiler
EXIT /B 1
:lbl_3___end