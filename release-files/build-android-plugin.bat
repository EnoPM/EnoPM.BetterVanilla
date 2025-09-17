@echo off
setLocal enableDelayedExpansion

set "ReleaseName=BetterVanilla"
set "ProjectNames=BetterVanilla"

set "Il2CppAutoInteropRepository=EnoPM/Il2CppAutoInterop"
set "Il2CppAutoInteropVersion=v1.1.0"
set "Il2CppAutoInteropRuntime=win-x64"

set "BepInExVersion=6.0.0"
set "BepInExBuildNumber=738"
set "BepInExBuildHash=af0cba7"

set "PluginOutputDirectory=%~1"
set "UnityProjectDirectory=%~2"

set "BepInExDownloadUrl=https://builds.bepinex.dev/projects/bepinex_be/%BepInExBuildNumber%/BepInEx-Unity.IL2CPP-win-x86-%BepInExVersion%-be.%BepInExBuildNumber%+%BepInExBuildHash%.zip"


set "AmongUsBepInExDownloadUrl=https://raw.githubusercontent.com/EnoPM/EnoPM.BetterVanilla/refs/heads/master/release-files/AmongUs.BepInEx.zip"

set "TempDirectory=%~dp0.build-plugin-temp"

set "BuildCacheDirectory=%~dp0.build-cache"

set "SolutionDir=%~dp0.."
set "BuildDirectory=%TempDirectory%\builds"
set "OutputDirectory=%~dp0output-android"
set "BepInExDirectory=%BuildCacheDirectory%\BepInEx-%BepInExVersion%-%BepInExBuildNumber%-%BepInExBuildHash%"
set "BepInExAmongUsZipPath=%~dp0AmongUs.BepInEx.zip"

set "Il2CppAutoInteropZipFile=Il2CppAutoInterop.%Il2CppAutoInteropRuntime%.zip"
set "Il2CppAutoInteropZipPath=%TempDirectory%\%Il2CppAutoInteropZipFile%"
set "Il2CppAutoInteropExecutableName=Il2CppAutoInterop-%Il2CppAutoInteropVersion%.exe"
set "Il2CppAutoInteropExecutablePath=%BuildCacheDirectory%\%Il2CppAutoInteropExecutableName%"
set "Il2CppAutoInteropDownloadUrl=https://github.com/%Il2CppAutoInteropRepository%/releases/download/%Il2CppAutoInteropVersion%/%Il2CppAutoInteropZipFile%"

set "Il2CppAutoInteropInputFiles=-i"

call :CreateDirectoryIfNotExist "%TempDirectory%"
call :CreateDirectoryIfNotExist "%BuildDirectory%"
call :CreateDirectoryIfNotExist "%OutputDirectory%"
call :CreateDirectoryIfNotExist "%BuildCacheDirectory%"

if not exist "%Il2CppAutoInteropExecutablePath%" (
    call :DownloadAndExtractZip "%Il2CppAutoInteropDownloadUrl%" "%TempDirectory%"
    if not exist "%TempDirectory%\Il2CppAutoInterop.exe" (
        echo [!] Error: File 'Il2CppAutoInterop.exe' not found in '%TempDirectory%'.
        exit /b 1
    )
    call :MoveFile "%TempDirectory%\Il2CppAutoInterop.exe" "%Il2CppAutoInteropExecutablePath%"
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
call :MakeIl2CppAutoInterop "%PluginOutputDirectory%"
call :DeleteDirectory "%TempDirectory%"

echo [+] Project built
endLocal
exit /b 0

:CleanupBepInExDirectory

echo [+] Cleaning up BepInEx directory
call :DeleteDirectoryIfExist "%BepInExDirectory%\BepInEx\interop"

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
setLocal
set "Il2CppAutoInteropOutputDirectory=%~1"

set "ProjectAutoInteropLog=%TempDirectory%\Il2CppAutoInterop.log"
if "%UnityProjectDirectory%" == "" (
    echo [+] Running Il2CppAutoInterop: '"%Il2CppAutoInteropExecutablePath%" -o "%Il2CppAutoInteropOutputDirectory%" -b "%BepInExDirectory%\BepInEx" %Il2CppAutoInteropInputFiles%'
    "%Il2CppAutoInteropExecutablePath%" -o "%Il2CppAutoInteropOutputDirectory%" -b "%BepInExDirectory%\BepInEx" %Il2CppAutoInteropInputFiles% > "%ProjectAutoInteropLog%" 2>&1
) else (
    if not exist "%UnityProjectDirectory%" (
        echo [!] Error: Unity project directory '%UnityProjectDirectory%' not found.
        exit /b 1
    )
    echo [+] Running Il2CppAutoInterop: '"%Il2CppAutoInteropExecutablePath%" -o "%Il2CppAutoInteropOutputDirectory%" -b "%BepInExDirectory%\BepInEx" -u "%UnityProjectDirectory%"  %Il2CppAutoInteropInputFiles%'
    "%Il2CppAutoInteropExecutablePath%" -o "%Il2CppAutoInteropOutputDirectory%" -b "%BepInExDirectory%\BepInEx" -u "%UnityProjectDirectory%"  %Il2CppAutoInteropInputFiles% > "%ProjectAutoInteropLog%" 2>&1
)

if not errorLevel 0 (
    echo [!] Error: Il2CppAutoInterop failed. See output below:
    type "%ProjectAutoInteropLog%"
    exit /b 1
) else (
    call :DeleteFileIfExist "%ProjectAutoInteropLog%"
)

endLocal
goto :eof

:BuildDotnetProject
setLocal
set "DotnetProjectName=%~1"
set "ProjectBuildLog=%TempDirectory%\%DotnetProjectName%.dotnet.log"

call :DeleteDirectoryIfExist "%BuildDirectory%\%DotnetProjectName%"

echo [+] Building project : '%DotnetProjectName%'
dotnet build "%SolutionDir%/%DotnetProjectName%/%DotnetProjectName%.csproj" ^
  --configuration Android ^
  --runtime win-x86 ^
  --output "%BuildDirectory%\%DotnetProjectName%" > "%ProjectBuildLog%" 2>&1
if errorLevel 1 (
    echo [!] Error: Build '%DotnetProjectName%' failed. See output below:
    type "%ProjectBuildLog%"
    exit /b 1
) else (
    call :DeleteFile "%ProjectBuildLog%"
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
powershell -Command "Expand-Archive -Force -Path '%ZipFilePath%' -DestinationPath '%ExtractDestinationDirectory%'"

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
