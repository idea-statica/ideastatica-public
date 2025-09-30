param (
    [Parameter(Mandatory = $true)]
    [string]$Source
)

$Destination = $PSScriptRoot

$SourceRoot = Join-Path $Source "clients\csharp"
$DestinationRoot = Join-Path $Destination "rcs-api\clients\csharp"

# List of child directories you want to copy
$ChildDirs = @(
    ".openapi-generator",
    "api",
    "docs",
    "src\IdeaStatiCa.RcsApi\Api",
    "src\IdeaStatiCa.RcsApi\Client"
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
    "src\IdeaStatiCa.RcsApi\IdeaStatiCa.RcsApi.csproj"
)

$ClientDestDir = Join-Path $DestinationRoot "src\IdeaStatiCa.RcsApi"

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
$DestinationRoot = Join-Path $Destination "rcs-api\clients\python"

# List of child directories you want to copy
$ChildDirs = @(
    ".openapi-generator",
    "docs",
    "ideastatica_rcs_api\api",
    "ideastatica_rcs_api\models"
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
    "ideastatica_rcs_api\*.py",
    "ideastatica_rcs_api\*.typed"
)

$ClientDestDir = Join-Path $DestinationRoot "ideastatica_rcs_api"

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