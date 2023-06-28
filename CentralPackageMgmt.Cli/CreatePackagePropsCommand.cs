
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace CentralPackageMgmt.Cli;

public class CreatePackagePropsCommand : Command<CreatePackagePropsCommand.Settings>
{
    public override int Execute([NotNull] CommandContext context, [NotNull] Settings settings)
    {
        var searchPath = settings.SearchPath ?? Directory.GetCurrentDirectory();
        var filePath = settings.OutputFile ?? "Directory.Packages.props";

        var projects = ProjectHelper.FindCsProjs(searchPath);

        var packages = projects
            .Select(p => Path.Combine(searchPath, p))
            .SelectMany(ProjectHelper.ReadProjectPackages)
            .ToLookup(n => n.Package)
            .OrderBy(n => n.Key)
            .ToArray();

        List<PackageInfo> packs = ResolvePackageVersions(packages);

        var file = ProjectHelper.WritePackagePropsFile(filePath, packs);

        AnsiConsole.WriteLine();
        AnsiConsole.WriteLine("File written at:");
        AnsiConsole.Write(new TextPath(file));
        AnsiConsole.WriteLine();

        if (!settings.SkipRemoval)
        {
            foreach (var p in projects)
            {
                AnsiConsole.MarkupLine($"[grey] Removing version info from project [white]{p}[/] [/]");

                ProjectHelper.RemoveProjectPackageVersions(p);
            }
        }

        return 0;
    }

    private static List<PackageInfo> ResolvePackageVersions(IGrouping<string, PackageInfo>[] packages)
    {
        var packs = new List<PackageInfo>(packages.Length);


        foreach (var package in packages)
        {
            var versions = package
                .Select(p => p.Version)
                .Distinct()
                .OrderByDescending(n => n)
                .ToArray();

            if (versions.Count() > 1)
            {
                var prompt = new SelectionPrompt<string>()
                        .Title($"Select preferred version of [green]{package.Key}[/]")
                        .AddChoices(versions);

                var selectedVersion = AnsiConsole.Prompt(prompt);

                packs.Add(new PackageInfo(package.Key, selectedVersion));

                AnsiConsole.MarkupLine($"[grey] Resolved [white]{package.Key}[/] to version [green]{selectedVersion}[/] [/]");
            }
            else
            {
                var selectedVersion = versions.Single();

                packs.Add(new PackageInfo(package.Key, selectedVersion));

                AnsiConsole.MarkupLine($"[grey] Resolved [white]{package.Key}[/] to version [green]{selectedVersion}[/] [/]");
            }


        }

        return packs;
    }

    public class Settings : BaseSettings
    {
        [Description("Path of props file to create. Defaults to Directory.Packages.props in current directory.")]
        [CommandOption("-o|--output")]
        public string? OutputFile { get; init; }

        [Description("Skip removing NuGet versions from csproj files.")]
        [CommandOption("-s|--skip-removal")]
        public bool SkipRemoval { get; init; }
    }
}