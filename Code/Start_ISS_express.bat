@cls
@echo off
echo NOTE : WHEN USING THIS BATCH PLEASE REMEMBER TO EDIT THE PATH IN THE CONFIG!
echo THANK YOU!
echo.  
echo.
"%ProgramFiles%\IIS Express\iisexpress.exe" /config:".\.vs\config\applicationhost.config" /siteid:2 /trace:error
pause