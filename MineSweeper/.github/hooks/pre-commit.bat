@echo off
setlocal enabledelayedexpansion

README_PATH="Minesweeper/README.md"

:: Get the commit message
set /p COMMIT_MSG=<.git\COMMIT_EDITMSG

:: Read the rest as description
(for /f "skip=1 delims=" %%i in (.git\COMMIT_EDITMSG) do (
    echo %%i
)) > temp_desc.txt

:: Append to README.md
echo. >> "$README_PATH"
echo ## **%COMMIT_MSG%** >> README.md
echo. >> "$README_PATH"
type temp_desc.txt >> README.md
echo. >> "$README_PATH"

del temp_desc.txt

:: Stage README.md
git add "$README_PATH"
