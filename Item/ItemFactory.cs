namespace Item;

using System;
using System.Diagnostics;
using System.IO;

public class ItemFactory
{
    private static readonly ItemFactory instance = new ItemFactory();
    public static ItemFactory Instance => instance;

    public object CreateItem(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            throw new ArgumentException("Path cannot be null or empty", nameof(path));
        }

        if (File.Exists(path))
        {
            return new FileItem(path);
        }
        else if (Directory.Exists(path))
        {
            return new DirectoryItem(path);
        }
        else
        {
            throw new FileNotFoundException("The specified path does not exist.");
        }
    }
}