@echo off
set YEAR=%DATE:~10,4%
set DAY=%DATE:~7,2%

set /A DAY=%DAY% + 1

if x%DAY:~1%x == xx set DAY=0%DAY%

copy /-y csharp\%YEAR%\Day00.cs csharp\%YEAR%\Day%DAY%.cs
copy /b NUL Data\%YEAR%\Day%DAY%.txt
copy /b NUL Data\%YEAR%\Day%DAY%_sample.txt
