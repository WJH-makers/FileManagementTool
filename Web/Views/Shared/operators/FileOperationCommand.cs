using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Views.Shared;

public class FileOperationCommand
{
    private readonly IFileOperationStrategy _strategy;

    public FileOperationCommand(IFileOperationStrategy strategy)
    {
        _strategy = strategy;
    }

    public async Task<FileOperationResult> Execute(List<IFormFile> files, string path)
    {
        // 权限检查
        if (!HasPermission(path))
        {
            return new FileOperationResult
            {
                Success = false,
                Message = "没有权限执行此操作。",
                FolderInfo = string.Empty
            };
        }

        return await _strategy.ExecuteAsync(files, path);
    }

    private bool HasPermission(string path)
    {
        return true;
    }
}