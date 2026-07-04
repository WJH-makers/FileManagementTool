<p align="center">
  <img src="https://capsule-render.vercel.app/api?type=waving&color=0:11998e,100:38ef7d&height=180&section=header&text=File%20Management%20Tool&fontSize=50&fontColor=ffffff&animation=fadeIn&fontAlignY=38&desc=Dockerized%20ASP.NET%20Core%20File%20Manager%20with%20ClamAV&descAlignY=55&descAlign=50" width="100%" />
</p>

<p align="center">
  <img src="https://img.shields.io/badge/C%23-ASP.NET%20Core-512BD4?style=flat-square&logo=dotnet" />
  <img src="https://img.shields.io/badge/Docker-Compose-2496ED?style=flat-square&logo=docker" />
  <img src="https://img.shields.io/badge/ClamAV-Antivirus-00A1E9?style=flat-square" />
  <img src="https://img.shields.io/badge/Pattern-Strategy-FF6F00?style=flat-square" />
  <img src="https://img.shields.io/badge/MVC-Architecture-green?style=flat-square" />
</p>

## 📋 Overview

A **Docker-based file management web app** built with **ASP.NET Core MVC** and **ClamAV antivirus**. Upload, scan, compress, decompress, and browse files through a clean web interface — all backed by a **Strategy Pattern** architecture for pluggable operations.

> **Why Strategy Pattern?** File operations (compress, scan, git push) share identical plumbing — select files, execute, report. The Strategy pattern decouples the operation logic from the controller, making it trivial to add new operations (encrypt, OCR, dedup) without touching existing code.

## 🚀 Quick Start

### Prerequisites

- .NET 8.0 SDK
- Docker Desktop
- Visual Studio 2022+ (optional)

### Run with Docker

```bash
cd App
docker-compose up -d
```

### Run without Docker

```bash
# Restore dependencies
dotnet restore

# Build
dotnet build

# Run web application
cd Web
dotnet run
```

Navigate to `http://localhost:5000` to access the file manager.

### API Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/` | Home - file browser |
| POST | `/Upload` | File upload |
| POST | `/Scan` | Virus scan |
| POST | `/Compress` | File compression |
| POST | `/Decompress` | File decompression |
| POST | `/GitLoad` | Upload to GitHub |

## ✨ Key Features

- **File Operations**: Upload, download, scan, compress, decompress, clean
- **Antivirus Integration**: ClamAV real-time virus scanning
- **Recursive Listing**: Browse directories with file metadata (size, permissions, icons)
- **Strategy Pattern**: Pluggable file operation strategies
- **Dockerized**: Full containerized deployment with docker-compose
- **Git Integration**: Upload files directly to GitHub repositories

## 🏗️ Architecture

```
FileManagementTool/
├── Web/                        # ASP.NET Core MVC web layer
│   ├── Controllers/
│   │   └── HomeController.cs   # File operation endpoints
│   ├── Views/                  # Razor views (Index, FileUpload, etc.)
│   ├── wwwroot/                # Static assets (CSS, JS, libs)
│   ├── Program.cs              # Application entry (Kestrel, ClamAV, MVC)
│   └── Web.csproj
├── Item/                       # Domain model library
│   ├── FileItem.cs             # File model (name, size, perms, icon)
│   ├── DirectoryItem.cs        # Directory model (recursive listing)
│   ├── DirectoryHelper.cs      # File system traversal
│   └── ItemFactory.cs          # Factory pattern for items
├── App/                        # Docker orchestration
│   └── docker-compose.yml      # Multi-service definition
├── Test/                       # Integration tests
└── FileManagementTool.sln
```

### Strategy Pattern

```
┌─────────────────────────────────────────────────┐
│            IFileOperationStrategy               │
├─────────────────────────────────────────────────┤
│  + ExecuteAsync(files, path): Task<Result>      │
├─────────────────────────────────────────────────┤
│         ▲           ▲           ▲               │
├─────────┼───────────┼───────────┼───────────────┤
│ Compress │  Decompress │   Scan    │  Git Load   │
│ Strategy │  Strategy   │ Strategy  │  Strategy   │
└──────────┴────────────┴───────────┴─────────────┘
```

## 🎓 Academic Context

This project was completed as the final project for the **.NET Programming** course at **Wuhan University**, demonstrating ASP.NET Core MVC, Docker containerization, design patterns, and antivirus integration.

---

<p align="center">
  <img src="https://capsule-render.vercel.app/api?type=waving&color=0:38ef7d,100:11998e&height=100&section=footer" width="100%" />
</p>
