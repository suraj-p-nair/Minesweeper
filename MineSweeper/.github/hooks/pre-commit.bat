@echo off
setlocal enabledelayedexpansion

:: Get the commit message
set /p COMMIT_MSG=<.git\COMMIT_EDITMSG

:: Read the rest as description
(for /f "skip=1 delims=" %%i in (.git\COMMIT_EDITMSG) do (
    echo %%i
)) > temp_desc.txt

:: Append to README.md
echo. >> README.md
echo ## **%COMMIT_MSG%** >> README.md
echo. >> README.md
type temp_desc.txt >> README.md
echo. >> README.md

del temp_desc.txt

:: Stage README.md
git add README.md
