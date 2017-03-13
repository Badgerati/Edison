param(
    [switch]
    $NoBuild
)

if (!$NoBuild) {
    cake
}

Clear-Host
.\Edison.Console\bin\Release\Edison.Console.exe --ef .\Edisonfile --tproj engine