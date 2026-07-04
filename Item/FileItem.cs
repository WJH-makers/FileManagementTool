using System;
using System.Drawing;
using System.IO;
using System.Text;


/*
对于文件而言：文件名 文件大小 文件路径 文件权限 文件编辑格式和内容
 */
public class FileItem
{
    public string Name { get; set; }
    public string Path { get; set; }
    public long Size { get; set; }
    public bool IsReadOnly { get; set; }
    public Icon FileIcon { get; set; }

    public FileItem(string path)
    {
        Path = path;
        Name = System.IO.Path.GetFileName(path);
        FileInfo fileInfo = new FileInfo(path);
        Size = fileInfo.Length;
        IsReadOnly = fileInfo.IsReadOnly;
        FileIcon = GetIconForFileOrFolder(path);
    }

    private Icon GetIconForFileOrFolder(string path)
    {
        Icon icon;
        icon = Icon.ExtractAssociatedIcon(path);
        return icon;
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
        result.AppendLine($"名称: {Name}");
        result.AppendLine($"路径: {Path}");
        result.AppendLine($"大小: {GetFormattedSize()}");
        result.AppendLine($"是否只读: {IsReadOnly}");
        return result.ToString();
    }
}