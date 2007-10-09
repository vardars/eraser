@echo off
rem Build with Digital Mars C/C++

rem Optimize, 386 instructions, huge memory model, stack size 0x2000
del *.obj
del *.exe
f:\dev\dm\bin\sc main.cpp list.cpp wipe.cpp random.cpp  -o eraserd.exe -ml
del *obj