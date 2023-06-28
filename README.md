# CentralPackageMgmt
A sample tool for aiding creation of Directory.Packages.props file based on the nugets discovered in csproj files.

Usage:
```
USAGE:
    CentralPackageMgmt.Cli.dll [OPTIONS] <COMMAND>

OPTIONS:
    -h, --help       Prints help information
    -v, --version    Prints version information

COMMANDS:
    list-projects
    list-packages
    create-props-file
```

Commands:
- list-projects - lists all *.csproj files that were discovered in the directory
- list-packages - lists all NuGet packages that were discovered in the projects. Displays info about version(s) found.
- create-props-file - creates `Directory.Packages.prop` file. Removes versions from csproj files, unless the option  `--skip-removal` is provided.
