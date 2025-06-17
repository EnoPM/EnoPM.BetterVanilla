@echo off
setLocal enableDelayedExpansion

set "ReleaseVersion=%1"
set "UnityProjectDirectory=%2"
if "%ReleaseVersion%"=="" (
    echo [!] Error: No version provided. Usage: create-release.bat v1.2.3
    exit /b 1
)
set "ReleaseName=BetterVanilla"
set "BepInExDownloadUrl=https://builds.bepinex.dev/projects/bepinex_be/735/BepInEx-Unity.IL2CPP-win-x86-6.0.0-be.735+5fef357.zip"

set "ProjectNames=BetterVanilla"
set "Il2CppAutoInteropVersion=v1.0.0"
set "Il2CppAutoInteropRuntime=win-x64"
set "AmongUsBepInExDownloadUrl=https://raw.githubusercontent.com/EnoPM/EnoPM.BetterVanilla/refs/heads/master/release-files/AmongUs.BepInEx.zip"

set "SolutionDir=%~dp0.."
set "TempDirectory=%~dp0.BetterVanillaBuild"
set "BuildDirectory=%TempDirectory%\builds"
set "OutputDirectory=%~dp0output"
set "BepInExDirectory=%TempDirectory%\BepInExFiles"
set "BepInExAmongUsZipPath=%~dp0AmongUs.BepInEx.zip"

set "Il2CppAutoInteropZipFile=Il2CppAutoInterop.%Il2CppAutoInteropRuntime%.zip"
set "Il2CppAutoInteropZipPath=%TempDirectory%\%Il2CppAutoInteropZipFile%"
set "Il2CppAutoInteropExecutableName=Il2CppAutoInterop.exe"
set "Il2CppAutoInteropExecutablePath=%TempDirectory%\%Il2CppAutoInteropExecutableName%"
set "Il2CppAutoInteropDownloadUrl=https://github.com/EnoPM/Il2CppAutoInterop/releases/download/%Il2CppAutoInteropVersion%/%Il2CppAutoInteropZipFile%"

set "Il2CppAutoInteropArgs=-b "%BepInExDirectory%\BepInEx" -o "%BepInExDirectory%\BepInEx\plugins""
set "Il2CppAutoInteropInputFiles=-i"
if not "%UnityProjectDirectory%"=="" (
    if exist "%UnityProjectDirectory%" (
        set "Il2CppAutoInteropArgs=%Il2CppAutoInteropArgs% -u "%UnityProjectDirectory%""
    ) else (
        echo [!] Error: Unity project directory '%UnityProjectDirectory%' not found.
        exit /b 1
    )
)

call :CreateDirectoryIfNotExist "%TempDirectory%"
call :CreateDirectoryIfNotExist "%BuildDirectory%"
call :CreateDirectoryIfNotExist "%OutputDirectory%"

if not exist "%Il2CppAutoInteropExecutablePath%" (
    call :DownloadAndExtractZip "%Il2CppAutoInteropDownloadUrl%" "%TempDirectory%"
)

if not exist %Il2CppAutoInteropExecutablePath% (
    echo [!] Error: Executable '%Il2CppAutoInteropExecutableName%' not found in '%Il2CppAutoInteropZipFile%'.
    exit /b 1
)

echo [+] Executable found : '%Il2CppAutoInteropExecutablePath%'

for %%p in (%ProjectNames%) do (
    call :BuildDotnetProject %%p
)

for %%p in (%ProjectNames%) do (
    if not exist "%BuildDirectory%\%%p\%%p.dll" (
        echo [!] Error: Assembly '%%p.dll' not found.
        exit /b 1
    )
    set "Il2CppAutoInteropInputFiles=%Il2CppAutoInteropInputFiles% "%BuildDirectory%\%%p\%%p.dll""
)
call :PrepareBepInExDirectory
call :MakeIl2CppAutoInterop
call :CreateRelease
call :DeleteDirectory "%TempDirectory%"

echo [+] Release created
endLocal
exit /b 0

:CreateRelease

echo [+] Creating BepInEx release zip archive
call :DeleteDirectoryIfExist "%BepInExDirectory%\BepInEx\interop"
call :DeleteFileIfExist "%OutputDirectory%\%ReleaseName%.%ReleaseVersion%.zip"
call :Zip "%BepInExDirectory%\*" "%OutputDirectory%\%ReleaseName%.%ReleaseVersion%.zip"
call :MoveFile "%BepInExDirectory%\BepInEx\plugins\*" "%OutputDirectory%\"

goto :eof

:PrepareBepInExDirectory

echo [+] Preparing BepInEx directory

if not exist "%BepInExDirectory%\BepInEx" (
    call :CreateDirectory "%BepInExDirectory%"
    call :DownloadAndExtractZip "%BepInExDownloadUrl%" "%BepInExDirectory%"
)

if not exist "%BepInExDirectory%\BepInEx\interop" (
    if not exist "%BepInExAmongUsZipPath%" (
        call :DownloadFile "%BepInExDownloadUrl%" "%BepInExAmongUsZipPath%"
    )
    call :Unzip "%BepInExAmongUsZipPath%" "%BepInExDirectory%\BepInEx"
)

goto :eof

:MakeIl2CppAutoInterop

echo [+] Running Il2CppAutoInterop: '"%Il2CppAutoInteropExecutablePath%" %Il2CppAutoInteropArgs% %Il2CppAutoInteropInputFiles%'
"%Il2CppAutoInteropExecutablePath%" %Il2CppAutoInteropArgs% %Il2CppAutoInteropInputFiles%

goto :eof

:BuildDotnetProject
setLocal
set "DotnetProjectName=%~1"

call :DeleteDirectoryIfExist "%BuildDirectory%\%DotnetProjectName%"

echo [+] Building project : '%DotnetProjectName%'
dotnet build "%SolutionDir%/%DotnetProjectName%/%DotnetProjectName%.csproj" ^
  --configuration Release ^
  --runtime win-x86 ^
  --output "%BuildDirectory%\%DotnetProjectName%"
if errorLevel 1 (
    echo [!] Error: Build '%DotnetProjectName%' failed.
    exit /b 1
)
echo [+] Success: Build succeeded : '%DotnetProjectName%'

endLocal
goto :eof

:DownloadAndExtractZip
setLocal

set "ZipDownloadUrl=%~1"
set "ExtractDirectoryPath=%~2"
set "ZipDestinationPath=%TempDirectory%\temp-zip"

call :CreateDirectoryIfNotExist "%ZipDestinationPath%"
set "ZipDestinationPath=%ZipDestinationPath%\downloaded.zip"

call :DownloadFile "%ZipDownloadUrl%" "%ZipDestinationPath%"
call :Unzip "%ZipDestinationPath%" "%ExtractDirectoryPath%"

del "%ZipDestinationPath%"

endLocal
goto :eof

:DownloadFile
setLocal

set "FileDownloadUrl=%~1"
set "FileDestinationPath=%~2"

echo [+] Downloading '%FileDownloadUrl%'
powershell -Command "Invoke-WebRequest -Uri '%FileDownloadUrl%' -OutFile '%FileDestinationPath%'"

endLocal
goto :eof

:Zip
setLocal
set "FilesDirectory=%~1"
set "ZipFilePath=%~2"

echo [+] Compressing '%FilesDirectory%'
powershell -Command "Compress-Archive -Path '%FilesDirectory%' -DestinationPath '%ZipFilePath%'"

endLocal
goto :eof

:Unzip
setLocal

set "ZipFilePath=%~1"
set "ExtractDestinationDirectory=%~2"

echo [+] Extracting '%ZipFilePath%'
powershell -Command "Expand-Archive -Path '%ZipFilePath%' -DestinationPath '%ExtractDestinationDirectory%'"

endLocal
goto :eof

:CreateDirectoryIfNotExist
setLocal
set "CreateDirectoryPath=%~1"

if not exist "%CreateDirectoryPath%" (
    call :CreateDirectory "%CreateDirectoryPath%"
)

endLocal
goto :eof

:CreateDirectory
setLocal
set "CreateDirectoryPath=%~1"

echo [+] Creating '%CreateDirectoryPath%' directory
mkdir "%CreateDirectoryPath%"

endLocal
goto :eof

:DeleteDirectoryIfExist
setLocal
set "DeleteDirectoryPath=%~1"

if exist "%DeleteDirectoryPath%" (
    call :DeleteDirectory "%DeleteDirectoryPath%"
)

endLocal
goto :eof

:DeleteDirectory
setLocal
set "DeleteDirectoryPath=%~1"

echo [-] Deleting directory : '%DeleteDirectoryPath%'
rmdir /s /q "%DeleteDirectoryPath%"

endLocal
goto :eof

:DeleteFileIfExist
setLocal
set "DeleteFilePath=%~1"

if exist "%DeleteFilePath%" (
    call :DeleteFile "%DeleteFilePath%"
)

endLocal
goto :eof

:DeleteFile
setLocal
set "DeleteFilePath=%~1"

echo [-] Deleting file : '%DeleteFilePath%'
del "%DeleteFilePath%"

endLocal
goto :eof

:MoveFile
setLocal
set "SourceFilePath=%~1"
set "DestinationFilePath=%~2"

echo [-] Moving file(s) : '%SourceFilePath%' to '%DestinationFilePath%'
move "%SourceFilePath%" "%DestinationFilePath%"

endLocal
goto :eof
