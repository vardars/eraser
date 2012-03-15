@echo off
IF NOT "%~1"=="" GOTO lbl_2_end
type _internal\build-bat-help
EXIT /B 1
:lbl_2_end
if not exist obj mkdir obj
if not exist out mkdir out
pushd obj
IF EXIST * DEL /F /Q *
popd
IF "%~1"=="mono" GOTO lbl_1_end
GOTO lbl_3_csc
:lbl_3_csc
pushd src
shift
shift
csc /nowarn:0162,0169,0219 /nologo /target:library /out:..\out\alglibnet2.dll %2 %3 %4 %5 %6 %7 %8 %9 %0 *.cs
IF NOT ERRORLEVEL 1 GOTO lbl_4
echo Error while compiling
popd
EXIT /B 1
:lbl_4
popd
GOTO lbl_3___end
:lbl_1_end
IF "%~3"=="" GOTO lbl_3_mono
echo Too many parameters specified
echo Did you enclose parameters to be passed to the compiler in double quotes?
EXIT /B 1
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
:lbl_3___end