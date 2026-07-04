using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Views.Shared;

public interface IFileOperationStrategy
{
    Task<FileOperationResult> ExecuteAsync(List<IFormFile> files, string path);
}