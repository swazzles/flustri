namespace Flustri.Core;

public static class DataHelper
{

    public static string GetServerDataDirectory()
    {
        return Path.Join(GetDataRootDirectory(), "flustri-server");
    }

    public static string GetClientDataDirectory()
    {
        return Path.Join(GetDataRootDirectory(), "flustri");
    }

    public static string GetDataRootDirectory()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        return path;
    }
}