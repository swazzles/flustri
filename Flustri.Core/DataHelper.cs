namespace Flustri.Core;

public static class DataHelper
{
    public static string GetDataFilePath(string relativeFilePath)
    {
        return Path.Join(GetDataDirectory(), relativeFilePath);
    }

    public static string GetDataDirectory()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        return Path.Join(path, "flustri");
    }
}