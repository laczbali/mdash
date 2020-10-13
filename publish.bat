echo Publishing to: %1
pause
cd Dashboard-SPA
call ng build --prod --build-optimizer=false
pause
cd ..\Dashboard.API
call dotnet publish -o %1
REM rename Dashboard.db Dashboard.db.old
REM call dotnet ef database update
REM move Dashboard.db %1
REM rename Dashboard.db.old Dashboard.db
