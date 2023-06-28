using Microsoft.Build.Construction;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using Microsoft.Extensions.FileSystemGlobbing;
using NuGet.Versioning;

namespace CentralPackageMgmt.Cli;

public record PackageInfo(string Package, string Version);

public class ProjectHelper
{
    public static IReadOnlyCollection<string> FindCsProjs(string searchDirectory)
    {
        var matcher = new Matcher();
        matcher.AddInclude("**/*.csproj");

        PatternMatchingResult result = matcher.Execute(
            new DirectoryInfoWrapper(
                new DirectoryInfo(searchDirectory)));

        var projects = result.Files.Select(f => f.Path).ToArray();

        return projects;
    }

    public static IReadOnlyCollection<PackageInfo> ReadProjectPackages(string csprojPath)
    {
        static PackageInfo ReadPackage(ProjectItemElement element)
        {
            var package = element.Include;
            var versionElement = element.Metadata.Single(c => c.ElementName == "Version");
            return new (package, versionElement.Value);
        }

        var projectRootElement = ProjectRootElement.Open(csprojPath);

        return projectRootElement.Items
            .Where(e => e.ItemType == "PackageReference")
            .Select(ReadPackage)
            .ToArray();
    }


    public static string WritePackagePropsFile(string filePath, IEnumerable<PackageInfo> packages)
    {
        var root = ProjectRootElement.Create(filePath);

        var propGroup = root.AddPropertyGroup();
        propGroup.AddProperty("ManagePackageVersionsCentrally", "true");
        propGroup.AddProperty("NoWarn", "$(NoWarn);NU1507");

        var itemGroup = root.AddItemGroup();

        foreach (var package in packages)
        {
            itemGroup.AddItem("PackageVersion", package.Package)
                     .AddMetadata("Version", package.Version, true);
        }

        root.Save();

        return root.ProjectFileLocation.File;
    }



    public static void RemoveProjectPackageVersions(string csprojPath)
    {
        static void RemovePackageVersion(ProjectItemElement element)
        {
            var versionElement = element.Metadata.Single(c => c.ElementName == "Version");

            element.RemoveChild(versionElement);
        }

        var projectRootElement = ProjectRootElement.Open(csprojPath);

        projectRootElement.Items
            .Where(e => e.ItemType == "PackageReference")
            .ForEach(RemovePackageVersion);

        projectRootElement.Save();
    }


}

