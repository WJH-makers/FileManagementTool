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
            var filePath = Path.Combine(savePath, file.FileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // Zip slip 防护：提取前校验每个条目路径是否安全
            using (var archive = ZipFile.OpenRead(filePath))
            {
                foreach (var entry in archive.Entries)
                {
                    var entryPath = Path.GetFullPath(Path.Combine(savePath, entry.FullName));
                    if (!entryPath.StartsWith(Path.GetFullPath(savePath), StringComparison.OrdinalIgnoreCase))
                    {
                        System.IO.File.Delete(filePath);
                        return new FileOperationResult
                        {
                            Success = false,
                            Message = "Zip slip attack detected: " + entry.FullName,
                            FolderInfo = string.Empty
                        };
                    }
                }
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