@echo off

rem Define the path to the .NET project
set "projectPath=C:\MyWorkSpace\MVC\DATABASERELATION-PAGINATION\DatabaseRelationship-Pagination\DatabaseRelationship-Pagination.csproj"

rem Define the output directory for the published files
set "outputPath=D:\Publish"

rem Define the configuration (Debug/Release)
set "configuration=Release"

rem Define the target framework (e.g., net6.0, net7.0)
set "framework=net7.0"

rem Define the remote server details
set "remoteUser=Silicon-Soft"
set "remoteHost=192.168.1.178"
set "remotePath=/Users/publish"
set "password=Silicon321"  rem Replace with your actual password

rem Run the publish command
dotnet publish "%projectPath%" -c %configuration% -f %framework% -o "%outputPath%"

rem Check if the publish command was successful
if %ERRORLEVEL%==0 (
    echo Project published successfully to %outputPath%
    
    rem Define the path to pscp.exe
    set "pscpPath=C:\Program Files\PuTTY\pscp.exe"  rem Update this path if necessary

    rem Construct the scp command
    "%pscpPath%" -r -pw %password% -batch "%outputPath%\*" %remoteUser%@%remoteHost%:%remotePath%

    rem Check if the pscp command was successful
    if %ERRORLEVEL%==0 (
        echo Files successfully copied to %remoteHost%:%remotePath%
    ) else (
        echo Failed to copy files to the remote server
    )
) else (
    echo Failed to publish the project
)

pause
