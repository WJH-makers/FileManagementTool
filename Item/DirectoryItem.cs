using System.Text;

namespace Item;

/*
对于文件夹: 文件夹名字 文件夹大小 文件夹权限 文件夹中是否包含某一个文件(递归搜索文件)

 */
public class DirectoryItem
{
    public string Path { get; set; }
    public string Name { get; set; }
    public long Size { get; set; }
    public bool IsHidden { get; set; }

    public DirectoryItem(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            throw new ArgumentException("Path cannot be null or empty", nameof(path));
        }

        Path = path;
        Name = System.IO.Path.GetFileName(path);
        if (Directory.Exists(path))
        {
            var dirInfo = new DirectoryInfo(path);
            Size = GetDirectorySize(dirInfo);
            IsHidden = (dirInfo.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden; // 是否隐藏
        }
        else
        {
            throw new FileNotFoundException("The specified path does not exist.");
        }
    }

    private long GetDirectorySize(DirectoryInfo directoryInfo)
    {
        long totalSize = 0;
        FileInfo[] files = directoryInfo.GetFiles();
        foreach (var file in files)
        {
            totalSize += file.Length;
        }

        DirectoryInfo[] subDirectories = directoryInfo.GetDirectories();
        foreach (var subDirectory in subDirectories)
        {
            totalSize += GetDirectorySize(subDirectory);
        }

        return totalSize;
    }

    public List<string> GetFileNames()
    {
        DirectoryHelper dirHelper = new DirectoryHelper();
        return dirHelper.GetAllFiles(Path);
    }


    public string GetFormattedSize()
    {
        if (Size >= 1024 * 1024 * 1024)
            return $"{Size / (1024 * 1024 * 1024):F2} GB";
        else if (Size >= 1024 * 1024)
            return $"{Size / (1024 * 1024):F2} MB";
        else if (Size >= 1024)
            return $"{Size / 1024:F2} KB";
        else
            return $"{Size} B";
    }

    public String Show()
    {
        var result = new StringBuilder();
        result.AppendLine("文件夹名称: " + Name);
        result.AppendLine("文件夹路径: " + Path);
        result.AppendLine("文件夹大小: " + GetFormattedSize());

        List<string> files = GetFileNames();
        result.AppendLine("全部文件有: ");
        foreach (var fileName in files)
        {
            result.AppendLine($"文件: {fileName}");
        }

        return result.ToString();
    }
}