using Spectre.Console.Cli;
using System.ComponentModel;

namespace CentralPackageMgmt.Cli;

public class BaseSettings : CommandSettings
{
    [Description("Path to search. Defaults to current directory.")]
    [CommandArgument(0, "[searchPath]")]
    public string? SearchPath { get; init; }
}
