using System.IO.Compression;

namespace WebApplication1.Views.Shared;

public class DecompressFileOperationStrategy : IFileOperationStrategy
{
    public async Task<FileOperationResult> ExecuteAsync(List<IFormFile> files, string path)
    {
        return await DecompressFiles(files, path);
    }

    private async Task<FileOperationResult> DecompressFiles(List<IFormFile> files, string path)
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

        // 设置保存解压文件的路径
        var savePath = Path.Combine(path, "decompressed_files");
        Directory.CreateDirectory(savePath);

        foreach (var file in files)
        {
            // Read into memory first for validation
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            ms.Position = 0;

            // Validate entries using memory stream
            using (var archive = new ZipArchive(ms, ZipArchiveMode.Read, leaveOpen: true))
            {
                foreach (var entry in archive.Entries)
                {
                    var entryPath = Path.GetFullPath(Path.Combine(savePath, entry.FullName));
                    if (!entryPath.StartsWith(Path.GetFullPath(savePath), StringComparison.OrdinalIgnoreCase))
                    {
                        return new FileOperationResult
                        {
                            Success = false,
                            Message = "Zip slip attack detected: " + entry.FullName,
                            FolderInfo = string.Empty
                        };
                    }
                }
            }

            // Write to disk and extract
            ms.Position = 0;
            var uniqueName = $"{Guid.NewGuid():N}_{file.FileName}";
            var filePath = Path.Combine(savePath, uniqueName);
            using (var fs = new FileStream(filePath, FileMode.Create))
            {
                await ms.CopyToAsync(fs);
            }

            ZipFile.ExtractToDirectory(filePath, savePath);
            System.IO.File.Delete(filePath);
        }

        return new FileOperationResult
        {
            Success = true,
            Message = "文件解压完成并保存成功。",
            FolderInfo = $"文件解压完成并保存在{savePath}"
        };
    }
}