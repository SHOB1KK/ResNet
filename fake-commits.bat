@echo off
setlocal enabledelayedexpansion

echo Fake commit script starting...
type nul >> dummy.txt

REM Создаём 99 фейковых коммитов с одинаковой датой (01.07.2025)
for /L %%i in (1,1,99) do (
    echo Fake commit %%i>> dummy.txt
    git add dummy.txt
    git commit --date="2025-07-01T12:00:00" -m "Fake commit %%i"
)

REM Финальный коммит: 1 августа 2025 в 11:00
echo Final commit>> dummy.txt
git add dummy.txt
git commit --date="2025-08-01T11:00:00" -m "Fake commit 100"

echo Done!
pause
