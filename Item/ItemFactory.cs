namespace Item;

using System;
using System.Diagnostics;
using System.IO;

public class ItemFactory
{
    private static readonly ItemFactory instance = new ItemFactory();
    private FileItem _fileItem;
    private DirectoryItem _directoryItem;
    public static ItemFactory Instance => instance;

    public object CreateItem(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            throw new ArgumentException("Path cannot be null or empty", nameof(path));
        }

        if (File.Exists(path))
        {
            if (_fileItem == null || _fileItem.Path != path)
            {
                _fileItem = new FileItem(path);
            }

            return _fileItem;
        }
        else if (Directory.Exists(path))
        {
            if (_directoryItem == null || _directoryItem.Path != path)
            {
                _directoryItem = new DirectoryItem(path);
            }

            return _directoryItem;
        }
        else
        {
            throw new FileNotFoundException("The specified path does not exist.");
        }
    }
}