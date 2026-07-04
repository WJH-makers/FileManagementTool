using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Views.Shared;

public class CleanFileOperationStrategy : IFileOperationStrategy
{
    private readonly string _basePath;

    public CleanFileOperationStrategy(IWebHostEnvironment env)
    {
        _basePath = Path.Combine(env.WebRootPath, "uploads", "cleaned");
    }

    public async Task<FileOperationResult> ExecuteAsync(List<IFormFile> files, string path)
    {
        return await CleanFiles(files, path);
    }

    private async Task<FileOperationResult> CleanFiles(List<IFormFile> files, string path)
    {
        if (files == null || files.Count == 0)
        {
            return new FileOperationResult
            {
                Success = false,
                Message = "没有选择文件进行上传。",
                FolderInfo = string.Empty
            };
        }

        foreach (var file in files)
        {
            if (file.Length > 0)
            {
                string uniqueName = $"{Guid.NewGuid():N}_{file.FileName}";
                string originalFilePath = Path.Combine(_basePath, uniqueName);
                using (var fileStream = new FileStream(originalFilePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                await CleanFile(originalFilePath);

                var cleanedFilePath = Path.Combine(path, Path.GetFileName(file.FileName));
                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                }

                System.IO.File.Copy(originalFilePath, cleanedFilePath, overwrite: true);
                if (System.IO.File.Exists(originalFilePath))
                {
                    System.IO.File.Delete(originalFilePath);
                }
            }
        }

        return new FileOperationResult
        {
            Success = true,
            Message = "文件清理完成并保存成功。",
            FolderInfo = $"文件清理完成并保存在{path}"
        };
    }

    private static async Task CleanFile(string filePath)
    {
        try
        {
            if (!System.IO.File.Exists(filePath))
            {
                Console.WriteLine($"文件 {filePath} 不存在。");
                return;
            }

            if (IsBinaryFile(filePath))
            {
                Console.WriteLine($"二进制文件 {filePath} 无需清理（如 PDF、图片等）。");
                return;
            }

            var encoding = Encoding.UTF8;
            string content = await System.IO.File.ReadAllTextAsync(filePath, encoding);

            content = CleanContent(content);
            await System.IO.File.WriteAllTextAsync(filePath, content, encoding);
            Console.WriteLine($"文件 {filePath} 已被清理。");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"清理文件 {filePath} 时发生错误: {ex.Message}");
        }
    }

    private static bool IsBinaryFile(string filePath)
    {
        var binaryExtensions = new[] { ".pdf", ".jpg", ".png", ".mp3", ".exe", ".docx" };
        var extension = Path.GetExtension(filePath)?.ToLower();
        return Array.Exists(binaryExtensions, ext => ext == extension);
    }

    private static string CleanContent(string content)
    {
        var lines = content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

        var cleanedLines = lines
            .Select(line => line.Trim())
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .Select(line => Regex.Replace(line, @"\s+", " "))
            .ToList();

        var nonEmptyContent = string.Join("\n", cleanedLines);

        nonEmptyContent = nonEmptyContent.Replace("\r\n", "\n");

        nonEmptyContent = Regex.Replace(nonEmptyContent, @"(\n){2,}", "\n");

        return nonEmptyContent;
    }
}