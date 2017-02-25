
Write-Host 'Packing Edison'

# == BUNDLE =======================================================

Write-Host "Copying binaries into package"
New-Item -ItemType Directory -Path ./Package

$bundleExtensions = @("*.dll", "*.exe", "*.config")

foreach ($extension in $bundleExtensions)
{
    Copy-Item -Path "./Edison.TestDriven/bin/Release/$extension" -Destination "./Package/" -Force
    Copy-Item -Path "./Edison.Console/bin/Release/$extension" -Destination "./Package/" -Force
    Copy-Item -Path "./Edison.Engine/bin/Release/$extension" -Destination "./Package/" -Force
    Copy-Item -Path "./Edison.Framework/bin/Release/$extension" -Destination "./Package/" -Force
    Copy-Item -Path "./Edison.GUI/bin/Release/$extension" -Destination "./Package/" -Force
    Copy-Item -Path "./Edison.Injector/bin/Release/$extension" -Destination "./Package/" -Force
}

Copy-Item -Path "./Images/icon.ico" -Destination "./Package" -Force
Write-Host "Binaries copied successfully"

# == ZIP =======================================================

Write-Host "Zipping package"
Push-Location "C:\Program Files\7-Zip\"
$zipName = "$env:BUILD_VERSION-Binaries.zip"

try
{
    .\7z.exe -tzip a "$env:WORKSPACE\$zipName" "$env:WORKSPACE\Package\*"
    Write-Host "Package zipped successfully"
}
finally
{
    Pop-Location
}

# == NUGET - FRAMEWORK =======================================================

Write-Host "Building NuGet Framework Package"
Push-Location "./nuget-packages/nuget/framework"

try
{
    mkdir lib
    cd lib
    mkdir net40
    Copy-Item -Path "$env:WORKSPACE/Edison.Framework/bin/Release/Edison.Framework.dll" -Destination "./net40" -Force
    cd ..
    (Get-Content Edison.Framework.dll.nuspec) | ForEach-Object { $_ -replace '\$version\$', $env:BUILD_VERSION } | Set-Content Edison.Framework.dll.nuspec
    nuget pack Edison.Framework.dll.nuspec
}
finally
{
    Pop-Location
}

# == NUGET - TESTDRIVEN ====================================================

Write-Host "Building NuGet TestDriven Package"
Push-Location "./nuget-packages/nuget/tdnet"

$drivenPath = "$env:WORKSPACE/Edison.TestDriven/bin/Release"

try
{
  mkdir tools
  Copy-Item -Path "$drivenPath/*.dll" -Destination "./tools" -Force
  Copy-Item -Path "$drivenPath/*.tdnet" -Destination "./tools" -Force
  (Get-Content Edison.TestDriven.nuspec) | ForEach-Object { $_ -replace '\$version\$', $env:BUILD_VERSION } | Set-Content Edison.TestDriven.nuspec
  nuget pack Edison.TestDriven.nuspec
}
finally
{
  Pop-Location
}

# == NUGET - CONSOLE =======================================================

Write-Host "Building NuGet Console Package"
Push-Location "./nuget-packages/nuget/console"

$consolePath = "$env:WORKSPACE/Edison.Console/bin/Release"

try
{
    mkdir tools
    Copy-Item -Path "$consolePath/*.dll" -Destination "./tools" -Force
    Copy-Item -Path "$consolePath/*.exe" -Destination "./tools" -Force
    Copy-Item -Path "$consolePath/*.config" -Destination "./tools" -Force
    (Get-Content Edison.Console.exe.nuspec) | ForEach-Object { $_ -replace '\$version\$', $env:BUILD_VERSION } | Set-Content Edison.Console.exe.nuspec
    nuget pack Edison.Console.exe.nuspec
}
finally
{
    Pop-Location
}

# == CHOCO =======================================================

Write-Host "Building Package Checksum"
Push-Location "$env:WORKSPACE"

try
{
    $checksum = (checksum -t sha256 -f $zipName)
    Write-Host "Checksum: $checksum"
}
finally
{
    Pop-Location
}

Write-Host "Building Choco Package"
Push-Location "./nuget-packages/choco"

try
{
    (Get-Content edison.nuspec) | ForEach-Object { $_ -replace '\$version\$', $env:BUILD_VERSION } | Set-Content edison.nuspec
    cd tools
    (Get-Content chocolateyinstall.ps1) | ForEach-Object { $_ -replace '\$version\$', $env:BUILD_VERSION } | Set-Content chocolateyinstall.ps1
    (Get-Content chocolateyinstall.ps1) | ForEach-Object { $_ -replace '\$checksum\$', $checksum } | Set-Content chocolateyinstall.ps1
    cd ..
    choco pack
}
finally
{
    Pop-Location
}

# =========================================================

Write-Host 'Edison Packed'