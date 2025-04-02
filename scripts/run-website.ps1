#!/usr/bin/env pwsh

# Using relative path from the script location and run Blazor app in src/Blazor.WebAsm.Demo

$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectPath = Join-Path $scriptPath "../src/Blazor.WebAsm.Demo"
$projectPath = Resolve-Path $projectPath

# Start the Blazor WebAssembly app

dotnet run --project $projectPath
