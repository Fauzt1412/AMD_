@echo off
echo Starting URL Shortener Application...
echo.

echo Checking prerequisites...

:: Check if .NET is installed
dotnet --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ERROR: .NET SDK is not installed or not in PATH
    echo Please install .NET 8 SDK from https://dotnet.microsoft.com/download
    pause
    exit /b 1
)

:: Check if Node.js is installed
node --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ERROR: Node.js is not installed or not in PATH
    echo Please install Node.js from https://nodejs.org/
    pause
    exit /b 1
)

echo Prerequisites check passed!
echo.

:: Start backend in a new window
echo Starting backend server...
start "Backend Server" cmd /k "cd Test.Server && dotnet run"

:: Wait a moment for backend to start
timeout /t 5 /nobreak >nul

:: Start frontend in a new window
echo Starting frontend development server...
start "Frontend Server" cmd /k "cd test.client && npm run dev"

echo.
echo Both servers are starting...
echo.
echo Backend will be available at: https://localhost:7282
echo Frontend will be available at: https://localhost:58242
echo Swagger UI will be available at: https://localhost:7282/swagger
echo.
echo Press any key to exit this script (servers will continue running)
pause >nul