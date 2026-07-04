using Item;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.IO;
namespace WebApplication1.Views.Shared;

public class ScanFileOperationStrategy : IFileOperationStrategy
{
    private readonly ItemFactory itemFactory = new ItemFactory();
    private int num = 0;
    private int infected_num = 0;
    private string Info = "";
    private string clamdInfo = "";


    public async Task<FileOperationResult> ExecuteAsync(List<IFormFile> files, string path)
    {
        return await ScanFiles(files);
    }

    private async Task<FileOperationResult> ScanFiles(List<IFormFile> files)
    {
        if (files != null && files.Count > 0)
        {
            string basePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            foreach (var file in files)
            {
                num += 1;
                if (file.Length > 0)
                {
                    string fileName = Path.GetFileName(file.FileName);
                    string uniqueName = $"{Guid.NewGuid():N}_{fileName}";
                    string path = Path.Combine(basePath, uniqueName);
                    Console.WriteLine(path);
                    using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None,
                               bufferSize: 8192))
                    using (var bufferedStream = new BufferedStream(stream))
                    {
                        await file.CopyToAsync(bufferedStream);
                    }

                    var scanResult = ScanWithClamAV(path);
                    if (scanResult.Contains("FOUND"))
                    {
                        infected_num += 1;
                    }

                    clamdInfo += scanResult;

                    if (System.IO.File.Exists(path))
                    {
                        FileItem fileItem = (FileItem)itemFactory.CreateItem(path);
                        Info += fileItem.Show();
                        System.IO.File.Delete(path);
                    }
                }
            }

            return new FileOperationResult
            {
                Success = true,
                Message = $"一共有{num}个文件，有{infected_num}个文件是病毒文件，有{num - infected_num}个文件是正常的",
                FolderInfo = $"一共有{num}个文件，有{infected_num}个文件是病毒文件，有{num - infected_num}个文件是正常的" + '\n' + clamdInfo +
                             Info,
            };
        }

        return new FileOperationResult
        {
            Success = false,
            FolderInfo = "没有选择文件，上传失败！",
            Message = "没有文件上传"
        };
    }
    public string ScanWithClamAV(string path)
    {
        Console.WriteLine("扫描文件/文件夹路径: " + path);
        ProcessStartInfo processStartInfo = new ProcessStartInfo
        {
            FileName = "clamdscan",
            Arguments = path,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        try
        {
            using (Process process = Process.Start(processStartInfo))
            {
                if (process.WaitForExit(30000))
                {
                    using (StreamReader reader = process.StandardOutput)
                    {
                        return reader.ReadToEnd();
                    }
                }
                else
                {
                    process.Kill();
                    clamdInfo += "Scan timed out after 30s\n";
                    return "FOUND";
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"无法执行 clamdscan：{ex.Message}");
            clamdInfo += $"ClamAV scan failed: {ex.Message}\n";
            return "FOUND";
        }
    }
}