---
title: Sessions Python Plugin Migration
description: Outlines the changes introduced to the SessionsPythonPlugin and provides steps for migrating.
author: SergeyMenshykh
ms.topic: conceptual
ms.author: semenshi
ms.date: 05/07/2025
ms.service: semantic-kernel
---

# Sessions Python Plugin Migration Guide

The `SessionsPythonPlugin` has been updated to use the latest version (2024-10-02-preview) of the Azure Code Interpreter Dynamic Sessions API. The new API has introduced breaking changes, which are reflected in the public API surface of the plugin.

This migration guide will help you migrate your existing code to the latest version of the plugin.

## The plugin initialization

The plugin constructor signature has changed to accept a `CancellationToken` as an argument for the `authTokenProvider` function. This change allows you to cancel the token generation process if needed:

```csharp
// Before
static async Task<string> GetAuthTokenAsync()
{
    return tokenProvider.GetToken();
}

// After
static async Task<string> GetAuthTokenAsync(CancellationToken ct)
{
    return tokenProvider.GetToken(ct);
}

var plugin = new SessionsPythonPlugin(settings, httpClientFactory, GetAuthTokenAsync);
```

## The UploadFileAsync method

The `UploadFileAsync` method signature has changed to better represent purpose of the parameters:

```csharp
// Before
string remoteFilePath = "your_file.txt";
string? localFilePath = "your_file.txt";

await plugin.UploadFileAsync(remoteFilePath: remoteFilePath, localFilePath: localFilePath);

// After
string remoteFileName = "your_file.txt";
string localFilePath = "your_file.txt";

await plugin.UploadFileAsync(remoteFileName: remoteFileName, localFilePath: localFilePath);
```

## The DownloadFileAsync method

Similarly, the `DownloadFileAsync` method signature has changed to better represent purpose of the parameters:

```csharp
// Before
string remoteFilePath = "your_file.txt";
await plugin.DownloadFileAsync(remoteFilePath: remoteFilePath);

// After
string remoteFileName = "your_file.txt";
await plugin.DownloadFileAsync(remoteFileName: remoteFileName);
```

## The ExecuteCodeAsync method

The `ExecuteCodeAsync` method signature has changed to provide a structured way for working with execution results:

```csharp
// Before
string result = await plugin.ExecuteCodeAsync(code: "print('Hello, world!')");

// After
SessionsPythonCodeExecutionResult result = await plugin.ExecuteCodeAsync(code: "print('Hello, world!')");
string status = result.Status;
string? executionResult = result.Result?.ExecutionResult;
string? stdout = result.Result?.StdOut;
string? stderr = result.Result?.StdErr;
```

## The SessionsRemoteFileMetadata class  
   
The `SessionsRemoteFileMetadata` model class, used by the `UploadFileAsync` and `ListFilesAsync` methods, has been updated to better represent the metadata of remote files and directories:

```csharp
// Before
SessionsRemoteFileMetadata file = await plugin.UploadFileAsync(...);
string fileName = file.Filename;
long fileSize = file.Size;
DateTime? lastModified = file.LastModifiedTime;

// After
SessionsRemoteFileMetadata file = await plugin.UploadFileAsync(...);
string name = file.Name;
long? size = file.SizeInBytes;
DateTime lastModified = file.LastModifiedAt;
```