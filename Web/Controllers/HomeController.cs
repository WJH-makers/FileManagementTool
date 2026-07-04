using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Item;
using System.IO;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Linq;
using System.IO.Compression;
using System.Text.Json;
using WebApplication1.Views.Shared;
using Microsoft.AspNetCore.Hosting;

namespace YourNamespace.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment webHostEnvironment;

        public HomeController(IWebHostEnvironment webHostEnvironment)
        {
            this.webHostEnvironment = webHostEnvironment;
        }

        private static readonly Dictionary<string, Func<IWebHostEnvironment, IFileOperationStrategy>> OperationStrategies =
            new Dictionary<string, Func<IWebHostEnvironment, IFileOperationStrategy>>
            {
                { "scan", _ => new ScanFileOperationStrategy() },
                { "clean", env => new CleanFileOperationStrategy(env) },
                { "compress", _ => new CompressFileOperationStrategy() },
                { "decompress", _ => new DecompressFileOperationStrategy() },
                { "git_load", _ => new GitLoadFileOperationStrategy() }
            };
        public IFileOperationStrategy GetFileOperationStrategy(string currentAction, IWebHostEnvironment env)
        {
            if (OperationStrategies.TryGetValue(currentAction, out var strategyFactory))
            {
                return strategyFactory(env);
            }
            else
            {
                throw new InvalidOperationException("无效的操作");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadFile(List<IFormFile> files, string currentAction, string pathHidden)
        {
            if (files != null)
            {
                var allowedExtensions = new[] { ".txt", ".pdf", ".doc", ".docx", ".jpg", ".png", ".zip" };
                foreach (var file in files)
                {
                    var ext = Path.GetExtension(file.FileName)?.ToLowerInvariant();
                    if (string.IsNullOrEmpty(ext) || !allowedExtensions.Contains(ext))
                    {
                        return BadRequest($"File type '{ext}' is not allowed.");
                    }
                }
            }

            // 路径合法性校验：防止路径穿越
            if (string.IsNullOrWhiteSpace(pathHidden) || pathHidden.Contains(".."))
            {
                return BadRequest("Invalid path");
            }
            var safePath = Path.GetFullPath(Path.Combine(webHostEnvironment.WebRootPath, pathHidden));
            if (!safePath.StartsWith(webHostEnvironment.WebRootPath, StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Path traversal detected");
            }

            // 获取操作策略并执行操作
            IFileOperationStrategy strategy = GetFileOperationStrategy(currentAction, webHostEnvironment);
            var command = new FileOperationCommand(strategy, webHostEnvironment);
            FileOperationResult result = await command.Execute(files, safePath);
            return Json(new 
            {
                showFileUpload = result.Success,
                message = result.Message,
                folderInfo = result.FolderInfo
            });
        }


        public IActionResult Index()
        {
            ViewData["ShowFileUpload"] = true;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}