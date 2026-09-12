@echo off
setlocal EnableExtensions
cd /d "%~dp0"

echo.
echo === Brewora mobile setup ===
echo Folder: %cd%
echo.

git pull origin dev 2>nul

where node >nul 2>&1
if errorlevel 1 (
  echo Node.js nahi mila. Install try kar raha hoon...
  where winget >nul 2>&1
  if errorlevel 1 (
    echo Winget bhi nahi hai. Ye page kholo, Node LTS install karo, phir is file ko dubara double-click karo:
    start https://nodejs.org
    pause
    exit /b 1
  )
  winget install -e --id OpenJS.NodeJS.LTS --accept-package-agreements --accept-source-agreements
  echo Node install ke baad naya terminal chahiye. Is window ko band karke run-mobile.bat dubara chalao.
  pause
  exit /b 0
)

echo Node:
node -v
echo.

cd /d "%~dp0mobile"
if not exist ".env" (
  echo VITE_API_BASE_URL=http://localhost:17421/api> ".env"
  echo VITE_RAZORPAY_KEY_ID=rzp_test_replace_me>> ".env"
)

echo npm install...
call npm install
if errorlevel 1 (
  echo npm install fail ho gaya.
  pause
  exit /b 1
)

echo.
echo Browser khulega: http://localhost:43123
echo API chalu honi chahiye: http://localhost:17421
echo Band karne ke liye is window mein Ctrl+C
echo.
start "" http://localhost:43123
call npm run dev
pause
