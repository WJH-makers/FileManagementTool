using System.IO.Compression;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Views.Shared;

public class CompressFileOperationStrategy : IFileOperationStrategy
{
    public async Task<FileOperationResult> ExecuteAsync(List<IFormFile> files, string path)
    {
        return await CompressFiles(files, path);
    }

    private async Task<FileOperationResult> CompressFiles(List<IFormFile> files, string path)
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

        // 设置保存压缩包的路径
        var savePath =
            Path.Combine(path, "compressed_files.zip");

        using (var fileStream = new FileStream(savePath, FileMode.Create))
        {
            using (var archive = new ZipArchive(fileStream, ZipArchiveMode.Create, true))
            {
                foreach (var file in files)
                {
                    var entry = archive.CreateEntry(file.FileName);
                    using (var entryStream = entry.Open())
                    using (var fileStreamToRead = file.OpenReadStream())
                    {
                        await fileStreamToRead.CopyToAsync(entryStream);
                    }
                }
            }
        }

        return new FileOperationResult
        {
            Success = true,
            Message = "文件压缩完成并保存成功。",
            FolderInfo = $"文件压缩完成并保存在{savePath}"
        };
    }
}