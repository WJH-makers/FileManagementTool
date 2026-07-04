<p align="center">
  <img src="https://capsule-render.vercel.app/api?type=waving&color=0:11998e,100:38ef7d&height=180&section=header&text=%E6%96%87%E4%BB%B6%E7%AE%A1%E7%90%86%E5%B7%A5%E5%85%B7&fontSize=50&fontColor=ffffff&animation=fadeIn&fontAlignY=38&desc=Docker%20%E5%8C%96%20ASP.NET%20Core%20%E6%96%87%E4%BB%B6%E7%AE%A1%E7%90%86%20%2B%20ClamAV&descAlignY=55&descAlign=50" width="100%" />
</p>

| 类别 | 技术栈 |
|------|--------|
| **框架** | ASP.NET Core MVC, .NET 8.0 |
| **架构** | 策略模式 |
| **基础设施** | Docker Compose, ClamAV |
| **前端** | Razor Views, Bootstrap |

## 📋 简介

基于 ASP.NET Core MVC 的 Docker 化文件管理 Web 应用。支持上传、扫描、压缩、解压、浏览文件，采用策略模式架构实现可插拔操作，集成 ClamAV 杀毒引擎。

## 🚀 快速开始

```bash
# Docker 部署
cd App && docker-compose up -d

# 或本地运行
dotnet restore && dotnet build
cd Web && dotnet run
# 访问 http://localhost:5000
```

## ✨ 功能特性

- **文件操作**：上传、下载、扫描、压缩、解压、清理
- **病毒扫描**：ClamAV 实时杀毒
- **策略模式**：可插拔操作架构，新增操作无需改控制器
- **Docker 化**：完整容器化部署
- **Git 集成**：直接上传文件到 GitHub 仓库

## 🏗️ 项目结构

```
FileManagementTool/
├── Web/                       # ASP.NET Core MVC 层
│   ├── Controllers/           # 控制器
│   ├── Views/                 # Razor 视图
│   ├── wwwroot/               # 静态资源
│   └── Program.cs
├── Item/                      # 领域模型
│   ├── FileItem.cs            # 文件模型
│   ├── DirectoryItem.cs       # 目录模型
│   └── DirectoryHelper.cs
├── App/                       # Docker 编排
│   └── docker-compose.yml
├── Test/                      # 集成测试
└── FileManagementTool.sln
```

```
┌─────────────────────────────────────┐
│       IFileOperationStrategy        │
├─────────────────────────────────────┤
│  + ExecuteAsync(files, path)        │
├──────────┬──────────┬───────────────┤
│  Compress │ Decompress│   Scan/Git   │
│  Strategy │ Strategy  │   Strategies │
└──────────┴──────────┴───────────────┘
```

## ❓ 常见问题

| 问题 | 回答 |
|------|------|
| **需要 Docker 吗？** | 推荐使用，无 Docker 需单独安装 ClamAV 并调整配置 |
| **如何添加新操作？** | 实现 `IFileOperationStrategy` → 注册 DI → 添加控制器接口和视图 |
| **可用于生产？** | 课程项目，生产需加认证、HTTPS、输入过滤 |

## 🔗 相关项目

- [FTP](/WJH-makers/FTP) — 底层 TCP 文件传输，与本 Web 文件管理器互补

## 🎓 课程背景

武汉大学计算机学院 · .NET 程序设计课程设计。

---

<p align="center">
  <img src="https://capsule-render.vercel.app/api?type=waving&color=0:38ef7d,100:11998e&height=100&section=footer" width="100%" />
</p>
