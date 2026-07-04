namespace Item;

using System;
using System.Diagnostics;
using System.IO;

public class DirectoryHelper
{
    public List<string> GetAllFiles(string directoryPath)
    {
        if (string.IsNullOrEmpty(directoryPath))
        {
            throw new ArgumentException("Directory path cannot be null or empty.", nameof(directoryPath));
        }

        if (!Directory.Exists(directoryPath))
        {
            throw new DirectoryNotFoundException($"The specified directory does not exist: {directoryPath}");
        }

        List<string> allFiles = new List<string>();
        GetFilesRecursively(directoryPath, allFiles);
        return allFiles;
    }

    private void GetFilesRecursively(string currentDirectory, List<string> fileList)
    {
        try
        {
            string[] files = Directory.GetFiles(currentDirectory);
            fileList.AddRange(files);
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine($"Access denied to directory: {currentDirectory}. Skipping. {ex.Message}");
        }

        try
        {
            string[] subDirectories = Directory.GetDirectories(currentDirectory);
            foreach (var subDirectory in subDirectories)
            {
                GetFilesRecursively(subDirectory, fileList);
            }
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine($"Access denied to subdirectory: {currentDirectory}. Skipping. {ex.Message}");
        }
    }
    

}