@echo off
echo Installing Git hooks...

:: Copy all hooks from .github/hooks to .git/hooks
xcopy /Y /Q ".github\hooks\*" ".git\hooks\" >nul

echo Git hooks installed successfully.
pause
