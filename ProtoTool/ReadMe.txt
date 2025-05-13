## 一、下载Proto工具
   - 1.网站：https://github.com/protocolbuffers/protobuf/releases
      选择CSharp并下载:
      (编译器)protoc-24.0-win64.zip
      (解析库)protobuf-24.0.zip

## 二、下载.NET SDK版本
   - 1.查看Unity .Net版本
      Unity - BuildSettings - Other Settings - Api Compatibility Level

   - 2.安装对应.NET SDK（一般用4.5）
      网站：https://dotnet.microsoft.com/zh-cn/download/dotnet-framework

   - 3.Unity对应版本API官网
      搜索：Manual - .NET profile support

## 三、生成解析数据的工具
   - 1.用vsStudio打开项目
     protobuf-24.0/csharp/src/Google.Protobuf.sln 

   - 2.配置Google.Protobuf
     vsStudio - 工具 - NuGet管理包 - 管理解决方案的NuGet程序包  - 下载插件：Microsoft.NETFramework.ReferenceAssemblies - 插件中勾选：Google.Protobuf

   - 3.修改NET版本
     解决方案 - Google.Protobuf - <TargetFrameworks>netstandard1.1;netstandard2.0;net45;net50</TargetFrameworks> - <TargetFrameworks>net45</TargetFrameworks>

   - 4.重新生成
     解决方案 - Google.Protobuf - 右键重新生成 - 右键解决方案打开文件夹/bin/Debug/net45/***（net45文件夹下的文件拷贝至Unity/Plugins中）