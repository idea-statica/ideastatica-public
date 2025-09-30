param (
    [Parameter(Mandatory = $true)]
    [string]$Source
)

$Destination = $PSScriptRoot

$SourceRoot = Join-Path $Source "clients\csharp"
$DestinationRoot = Join-Path $Destination "connection-api\clients\csharp"

# List of child directories you want to copy
$ChildDirs = @(
    ".openapi-generator",
    "api",
    "docs",
    "src\IdeaStatiCa.ConnectionApi\Api",
    "src\IdeaStatiCa.ConnectionApi\Client"
)

foreach ($Child in $ChildDirs) {
    $SourcePath      = Join-Path $SourceRoot $Child
    $DestinationPath = Join-Path $DestinationRoot $Child

    $DestinationParent = Split-Path $DestinationPath -Parent

    # Copy directory
    Copy-Item -Path $SourcePath -Destination $DestinationParent -Recurse -Force
    Write-Host "Copied '$SourcePath' to '$DestinationParent'"
}

$ChildFiles = @(
    "src\IdeaStatiCa.ConnectionApi\IdeaStatiCa.ConnectionApi.csproj"
)

$ClientDestDir = Join-Path $DestinationRoot "src\IdeaStatiCa.ConnectionApi"

foreach ($Child in $ChildFiles) {
    $SourcePath      = Join-Path $SourceRoot $Child
    Copy-Item -Path $SourcePath -Destination $ClientDestDir -Force
}

$ChildFiles = @(
    "README.md"
)

$ClientDestDir = Join-Path $DestinationRoot ""

foreach ($Child in $ChildFiles) {
    $SourcePath      = Join-Path $SourceRoot $Child
    Copy-Item -Path $SourcePath -Destination $ClientDestDir -Force
}

$SourceRoot = Join-Path $Source "clients\python"
$DestinationRoot = Join-Path $Destination "connection-api\clients\python"

# List of child directories you want to copy
$ChildDirs = @(
    ".openapi-generator",
    "docs",
    "ideastatica_connection_api\api",
    "ideastatica_connection_api\models"
)

foreach ($Child in $ChildDirs) {
    $SourcePath      = Join-Path $SourceRoot $Child
    $DestinationPath = Join-Path $DestinationRoot $Child

    $DestinationParent = Split-Path $DestinationPath -Parent

    # Copy directory
    Copy-Item -Path $SourcePath -Destination $DestinationParent -Recurse -Force
    Write-Host "Copied '$SourcePath' to '$DestinationParent'"
}

$ChildFiles = @(
    "ideastatica_connection_api\*.py",
    "ideastatica_connection_api\*.typed"
)

$ClientDestDir = Join-Path $DestinationRoot "ideastatica_connection_api"

foreach ($Child in $ChildFiles) {
    $SourcePath      = Join-Path $SourceRoot $Child
    Copy-Item -Path $SourcePath -Destination $ClientDestDir -Force
}

$ChildFiles = @(
    "README.md",
    "setup.*"
)

$ClientDestDir = Join-Path $DestinationRoot ""

foreach ($Child in $ChildFiles) {
    $SourcePath      = Join-Path $SourceRoot $Child
    Copy-Item -Path $SourcePath -Destination $ClientDestDir -Force
}