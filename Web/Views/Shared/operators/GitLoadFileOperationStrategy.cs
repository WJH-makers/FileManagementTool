using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Views.Shared;

public class GitLoadFileOperationStrategy : IFileOperationStrategy
{
    public async Task<FileOperationResult> ExecuteAsync(List<IFormFile> files, string path)
    {
        return await git_loads(files);
    }

    private async Task<FileOperationResult> git_loads(List<IFormFile> files)
    {
        var token = Environment.GetEnvironmentVariable("GITHUB_TOKEN") ?? ""; // 从环境变量读取
        var repoOwner = "WJH-makers"; // 替换为你的 GitHub 用户名
        var repoName = "dotnet"; // 替换为你的仓库名
        var branch = "main"; // 使用仓库的默认分支，如 "main" 或 "master"
        if (files == null || files.Count == 0)
        {
            return new FileOperationResult
            {
                Success = false,
                Message = "没有选择文件进行上传。",
                FolderInfo = string.Empty
            };
        }

        var apiUrl = $"https://api.github.com/repos/{repoOwner}/{repoName}/contents/";

        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("User-Agent", "dotnet"); // 正确添加 User-Agent
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            foreach (var file in files)
            {
                var fileName = file.FileName;
                var fileContent = await ReadFileContent(file); // 获取文件内容并转换为 Base64 编码
                var fileUrl = $"{apiUrl}{fileName}";
                var sha = "";
                // 检查文件是否已存在
                var fileRequestMessage = new HttpRequestMessage(HttpMethod.Get, fileUrl);
                var fileResponse = await client.SendAsync(fileRequestMessage);

                if (fileResponse.IsSuccessStatusCode)
                {
                    // 如果文件存在，获取文件的 sha 值
                    var fileData =
                        JsonSerializer.Deserialize<JsonElement>(await fileResponse.Content.ReadAsStringAsync());
                    sha = fileData.GetProperty("sha").GetString();
                }

                var jsonData = new
                {
                    message = $"上传文件: {fileName}",
                    committer = new
                    {
                        name = "WJH", // 提交者姓名
                        email = "136443811+WJH-makers@users.noreply.github.com" // 提交者邮箱
                    },
                    content = fileContent,
                    sha = string.IsNullOrEmpty(sha) ? null : sha, // 如果 sha 为空，表示是新文件
                    branch = branch
                };

                var json = JsonSerializer.Serialize(jsonData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var requestMessage = new HttpRequestMessage(HttpMethod.Put, fileUrl)
                {
                    Content = content
                };

                var response = await client.SendAsync(requestMessage);
                if (!response.IsSuccessStatusCode)
                    return new FileOperationResult
                    {
                        Success = false,
                        Message = $"Error: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}",
                        FolderInfo = string.Empty
                    };
            }
        }

        return new FileOperationResult
        {
            Success = true,
            Message = "成功上传到github中",
            FolderInfo = "文件上传到 GitHub 完成并保存成功"
        };
    }

    private async Task<string> ReadFileContent(IFormFile file)
    {
        using (var memoryStream = new MemoryStream())
        {
            await file.CopyToAsync(memoryStream);
            var fileBytes = memoryStream.ToArray();
            return Convert.ToBase64String(fileBytes); // 将文件内容转换为 Base64 编码
        }
    }
}
