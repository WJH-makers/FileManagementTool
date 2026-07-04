using System;
using Item;
using System.Diagnostics;
using System.IO;

class Program
{
    static void Main()
    {
        ItemFactory itemFactory = new ItemFactory();
        string path = @"C:\Users\wjh19\Desktop\test.txt";
        ScanWithClamAV(path);
        if (File.Exists(path))
        {
            FileItem fileItem = (FileItem)itemFactory.CreateItem(path);
            fileItem.Show();
        }

        if (Directory.Exists(path))
        {
            DirectoryItem directoryItem = (DirectoryItem)itemFactory.CreateItem(path);
            directoryItem.Show();
        }

        string ScanWithClamAV(string Path)
        {
            Console.WriteLine("扫描文件/文件夹路径: " + Path);
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                FileName = "clamdscan",
                Arguments = Path,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            try
            {
                using (Process process = Process.Start(processStartInfo))
                {
                    using (StreamReader reader = process.StandardOutput)
                    {
                        string output = reader.ReadToEnd();
                        return output;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"无法执行 clamdscan：{ex.Message}");
            }

            return "";
        }
    }
}