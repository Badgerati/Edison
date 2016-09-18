$ErrorActionPreference = 'Stop';

$packageName  = 'Edison'
$toolsDir     = "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"
$url          = 'https://github.com/Badgerati/Edison/releases/download/v$version$/$version$-Binaries.zip'
$checksum     = '$checksum$'
$checksumType = 'sha256'

$packageArgs = @{
  PackageName   = $packageName
  UnzipLocation = $toolsDir
  Url           = $url
  Checksum      = $checksum
  ChecksumType  = $checksumType
}

Install-ChocolateyZipPackage @packageArgs