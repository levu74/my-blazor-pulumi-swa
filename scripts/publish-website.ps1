
param(
    [Parameter(ValueFromRemainingArguments=$true)]
    $AdditionalArgs
)

# Using relative path from the script location and publish Blazor app in src/Blazor.WebAsm.Demo

$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectPath = Join-Path $scriptPath "..\src\Blazor.WebAsm.Demo"
$projectPath = Resolve-Path $projectPath

$publishDir = Join-Path $scriptPath "../dist/Blazor.WebAsm.Demo"


# Publish the Blazor WebAssembly app 

dotnet publish $projectPath -c Release  -o $publishDir -p:PublishProvider=FileSystem $AdditionalArgs
