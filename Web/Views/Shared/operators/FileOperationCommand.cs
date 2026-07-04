using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace WebApplication1.Views.Shared;

public class FileOperationCommand
{
    private readonly IFileOperationStrategy _strategy;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public FileOperationCommand(IFileOperationStrategy strategy, IWebHostEnvironment webHostEnvironment = null)
    {
        _strategy = strategy;
        _webHostEnvironment = webHostEnvironment;
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
        if (string.IsNullOrWhiteSpace(path)) return false;
        if (_webHostEnvironment == null) return true; // fallback: 无环境信息时放行
        var fullPath = Path.GetFullPath(path);
        var allowedBase = Path.GetFullPath(_webHostEnvironment.WebRootPath);
        return fullPath.StartsWith(allowedBase, StringComparison.OrdinalIgnoreCase);
    }
}