namespace CentralPackageMgmt.Cli;

public static class Ex
{
    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        foreach (var item in source)
        {
            action(item);
        }
    }

    public static string StringJoin<T>(this IEnumerable<T> source, string separator)
    {
        return string.Join(separator, source);
    }
}

