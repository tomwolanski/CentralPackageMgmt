using Spectre.Console;
using Spectre.Console.Cli;
using System;
using System.Diagnostics.CodeAnalysis;

namespace CentralPackageMgmt.Cli;

public class ListProjectsCommand : Command<ListProjectsCommand.Settings>
{
    public override int Execute([NotNull] CommandContext context, [NotNull] Settings settings)
    {
        var searchPath = settings.SearchPath ?? Directory.GetCurrentDirectory();

        var projects = ProjectHelper.FindCsProjs(searchPath);

        var table = new Table();
        table.AddColumn("Project");

        foreach (var project in projects)
        {
            var path = new TextPath(project)
                .LeafColor(Color.CadetBlue_1)
                .RootColor(Color.Green)
                .StemColor(Color.Olive)
                .SeparatorColor(Color.DarkRed_1);

            table.AddRow(path);
        }

        AnsiConsole.Write(table);

        return 0;
    }

    public class Settings : BaseSettings
    { }
}
