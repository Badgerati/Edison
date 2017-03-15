param(
    [switch]
    $NoBuild
)

if (!$NoBuild) {
    cake .\build-debug.cake
}

Clear-Host
.\Edison.Console\bin\Debug\Edison.Console.exe --ef .\Edisonfile --tproj engine