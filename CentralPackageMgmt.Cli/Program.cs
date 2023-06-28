using Spectre.Console.Cli;

namespace CentralPackageMgmt.Cli;

internal class Program
{
    static int Main(string[] args)
    {
        var app = new CommandApp();
        app.Configure(config =>
        {
            config.AddCommand<ListProjectsCommand>("list-projects");
            config.AddCommand<ListPackagesCommand>("list-packages");
            config.AddCommand<CreatePackagePropsCommand>("create-props-file");
        });

        return app.Run(args);
    }
}
