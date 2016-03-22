$ErrorActionPreference = 'Stop';

$packageName= 'Edison'
$toolsDir   = "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"
$url        = 'https://github.com/Badgerati/Edison/releases/download/v$version$/$version$-Binaries.zip'

$packageArgs = @{
  packageName   = $packageName
  unzipLocation = $toolsDir
  url           = $url
}

Install-ChocolateyZipPackage @packageArgs