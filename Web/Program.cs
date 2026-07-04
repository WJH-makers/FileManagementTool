using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// 配置 Kestrel 服务器选项
builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = 100 * 1024 * 1024; // 100MB
});

builder.Services.Configure<KestrelServerOptions>(options =>
{
    // 设置请求超时时间（例如 10 分钟）
    options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(10);
    options.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(10);
});

// 注册 MVC 控制器服务
builder.Services.AddControllersWithViews();

var app = builder.Build();

// 在应用启动时启动 clamd
StartClamd();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

if (!builder.Environment.IsEnvironment("Docker"))
{
    app.UseHttpsRedirection();
}
app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

void StartClamd()
{
    try
    {
        string clamdPath = builder.Configuration.GetValue<string>("ClamAV:ClamdPath") ?? "C:\\Program Files\\ClamAV\\clamd.exe";
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = clamdPath,
            Arguments = "",
            CreateNoWindow = true,
            UseShellExecute = false
        };
        Process process = Process.Start(startInfo);
        if (process != null)
        {
            Console.WriteLine("Clamd has been started successfully.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error starting clamd: {ex.Message}");
    }
}