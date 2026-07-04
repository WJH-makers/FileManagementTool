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

namespace YourNamespace.Controllers
{
    public class HomeController : Controller
    {
        private static readonly Dictionary<string, Func<IFileOperationStrategy>> OperationStrategies =
            new Dictionary<string, Func<IFileOperationStrategy>>
            {
                { "scan", () => new ScanFileOperationStrategy() },
                { "clean", () => new CleanFileOperationStrategy() },
                { "compress", () => new CompressFileOperationStrategy() },
                { "decompress", () => new DecompressFileOperationStrategy() },
                { "git_load", () => new GitLoadFileOperationStrategy() }
            };
        public IFileOperationStrategy GetFileOperationStrategy(string currentAction)
        {
            if (OperationStrategies.TryGetValue(currentAction, out var strategyFactory))
            {
                return strategyFactory();
            }
            else
            {
                throw new InvalidOperationException("无效的操作");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(List<IFormFile> files, string currentAction, string pathHidden)
        {
            // 获取操作策略并执行操作
            IFileOperationStrategy strategy = GetFileOperationStrategy(currentAction);
            var command = new FileOperationCommand(strategy);
            FileOperationResult result = await command.Execute(files, pathHidden);
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