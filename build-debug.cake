var target = Argument("target", "Default");
var configuration = Argument("configuration", "Debug");
var edisonSln = "./Edison.sln";

Task("Restore Packages")
    .Does(() =>
{
    NuGetRestore(edisonSln);
});

Task("Build")
    .IsDependentOn("Restore Packages")
    .Does(() =>
{
    MSBuild(edisonSln, settings =>
        settings.SetConfiguration(configuration)
                .SetVerbosity(Verbosity.Minimal));
});

Task("Unit Tests")
    .IsDependentOn("Build")
    .Does(() =>
{
    NUnit("**/bin/Debug/*.Test.dll", new NUnitSettings {
        ShadowCopy = false,
        NoLogo = true,
        ToolPath = "./packages/NUnit.Runners.2.6.4/tools/nunit-console.exe"
    });
});

Task("Default")
    .IsDependentOn("Unit Tests");

RunTarget(target);