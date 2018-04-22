#tool "nuget:?package=NUnit.ConsoleRunner"

var target              = Argument("target", "Default");
var configuration       = Argument("configuration", "Release");
var solutions           = GetFiles("./**/*.sln");
var solutionPaths       = solutions.Select(solution => solution.GetDirectory());


Task("Default")
  .Does(() =>
{
  Information("Hello World!");
});


Task("Restore")
    .Does(() =>
{
    // Restore all NuGet packages.
    foreach(var solution in solutions)
    {
        Information("Restoring {0}...", solution);
        NuGetRestore(solution);
    }
});

Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
   .Does(() =>
   {
       foreach(var solution in solutions)
       {
           Information("Building {0}", solution);
           MSBuild(solution, settings =>
            settings.SetPlatformTarget(PlatformTarget.MSIL)
             //   .WithProperty("TreatWarningsAsErrors","true")
                .WithTarget("Build")
                .SetConfiguration(configuration));
        
       }

   });


Task("Clean")
    .Does(() =>
{
    // Clean solution directories.
    foreach(var path in solutionPaths)
    {
        Information("Cleaning {0}", path);
        CleanDirectories(path + "/**/bin/" + configuration);
        CleanDirectories(path + "/**/obj/" + configuration);
    }
});


Task("Test")  
    .IsDependentOn("Build")
    .Does(() =>
    {
        var projects = GetFiles("./**/*test.dll");

        Information("Found following dlls:");
        Information("{0}", string.Join("\r\n", projects));
        Information("");


        foreach(var project in projects)
        {
           Information("Runnning tests for {0}", project);
           NUnit3(project.ToString());
        }
    });

RunTarget(target);
