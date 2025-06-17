@echo off
setlocal enabledelayedexpansion

set "configuration=Release"

set "projects=BetterVanilla"
set "SolutionDir=%CD%"
set "PostCompilerExecutable=D:\GameDevelopment\Il2CppAutoInterop-publish\Il2CppAutoInterop.exe"
set "UnityProject=D:\Projects\BetterVanilla UnityEditor\My project"
set "BepInExDirectory=D:\GameDevelopment\AmongUs\DevFiles\BepInEx"
set "OutputDir=%BepInExDirectory%\plugins"

set "PostCompilerArgs=-b "%BepInExDirectory%" -o "%OutputDir%""

if not "%UnityProject%"=="" (
    if exist "%UnityProject%" (
        set "PostCompilerArgs=!PostCompilerArgs! -u "%UnityProject%""
    )
)

if exist "%OutputDir%" (
    echo Cleaning output directory
    del /q "%OutputDir%\*"
    for /d %%d in ("%OutputDir%\*") do rd /s /q "%%d"
) else (
    mkdir "%OutputDir%"
)

set "InputFiles=-i"

for %%p in (%projects%) do (
    echo ========================
    echo Building %%p ...
    echo ========================
    dotnet build "%SolutionDir%\%%p\%%p.csproj" --configuration %configuration%
    if errorlevel 1 (
        echo Build failed for %%p
        exit /b 1
    ) else (
        echo Build succeeded for %%p
        set "dllPath=%SolutionDir%\%%p\bin\%configuration%\net6.0\%%p.dll"
        set "InputFiles=!InputFiles! "!dllPath!""
    )
)

echo ========================
echo All projects built successfully.
echo Running post compiler:
echo "%PostCompilerExecutable%" %PostCompilerArgs% %InputFiles%
"%PostCompilerExecutable%" %PostCompilerArgs% %InputFiles%
