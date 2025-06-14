namespace BlockBlast_2._0.utils;

using System.Reflection;

public static class ResourceHelper
{
    public static Stream? GetEmbeddedResourceStream(string resourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        return assembly.GetManifestResourceStream(resourceName);
    }

    public static void ListAllResourceNames()
    {
        var assembly = Assembly.GetExecutingAssembly();
        foreach (var name in assembly.GetManifestResourceNames())
            Console.WriteLine(name);
    }
}
