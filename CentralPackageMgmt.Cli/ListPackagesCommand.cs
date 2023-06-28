using Spectre.Console;
using Spectre.Console.Cli;
using System.Diagnostics.CodeAnalysis;

namespace CentralPackageMgmt.Cli;

public class ListPackagesCommand : Command<ListPackagesCommand.Settings>
{
    public override int Execute([NotNull] CommandContext context, [NotNull] Settings settings)
    {
        var searchPath = settings.SearchPath ?? Directory.GetCurrentDirectory();

        var packages = ProjectHelper.FindCsProjs(searchPath)
            .Select(p => Path.Combine(searchPath, p))
            .SelectMany(ProjectHelper.ReadProjectPackages)
            .ToLookup(n => n.Package);

        var table = new Table();
        table.AddColumn("Package");
        table.AddColumn("Used in projects");
        table.AddColumn("Versions");

        foreach (var package in packages.OrderBy(n => n.Key))
        {
            var usedInCount = package.Count();

            //var versions = package
            //    .Select(v => v.Version)
            //    .Distinct()
            //    .OrderDescending()
            //    .Select(v => new Text(v.ToString()))
            //    .ToArray();


            var versions = package
                .Select(v => v.Version)
                .Distinct()
                .OrderByDescending(n => n)
                .StringJoin(", ");
                

            table.AddRow(
                new Text(package.Key), 
                new Text(usedInCount.ToString()), 
                new Text(versions));

        }

        AnsiConsole.Write(table);

        return 0;
    }

    public class Settings : BaseSettings
    { }
}
