$ErrorActionPreference = 'Stop';

$packageName= 'Edison'
$toolsDir   = "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"
$url        = 'https://github.com/Badgerati/Edison/releases/download/v1.0.0.6/1.0.0.6-Binaries.zip'

$packageArgs = @{
  packageName   = $packageName
  unzipLocation = $toolsDir
  url           = $url
}

Install-ChocolateyZipPackage @packageArgs