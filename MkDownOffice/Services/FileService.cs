using Microsoft.FluentUI.AspNetCore.Components;

using MkDownOffice.Contracts;
using MkDownOffice.Models;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MkDownOffice.Services;

public class FileService : IFileService
{
  public async Task<Folder> OpenFolderAsync(string path)
  {
    var dir = new DirectoryInfo(path);
    if (!dir.Exists) throw new DirectoryNotFoundException();
    var folder = new Folder
    {
      Name = dir.Name,
      Path = dir.FullName,
      Folders = dir.GetDirectories()
                   .AsQueryable()
                   .OrderBy(x => x.Name)
                   .Select(x => x.Name)
                   .ToList(),
      Files = dir.GetFiles()
                 .AsQueryable()
                 .OrderBy(x => x.Name)
                 .Select(x => x.Name)
                 .ToList()
    };
    return await Task.FromResult(folder);
  }

  public async Task<MarkdownFile> OpenFileAsync(string path)
  {
    var info = new FileInfo(path);
    if (!info.Exists) throw new FileNotFoundException();

    var mdFile = new MarkdownFile();
    mdFile.Name = info.Name;
    mdFile.Path = info.FullName;
    using var reader = info.OpenText();
    mdFile.Markdown = await reader.ReadToEndAsync();

    return mdFile;
  }

  public async Task SaveFileAsync(MarkdownFile mdFile)
  {
    var info = new FileInfo(mdFile.Path);
    bool isBlankLine = false;

    using var writer = info.CreateText();
    foreach (var l in mdFile.Markdown.Split('\n'))
    {
      if (isBlankLine && string.IsNullOrWhiteSpace(l))
      {
        continue;
      }
      if (string.IsNullOrWhiteSpace(l))
      {
        isBlankLine = true;
      }
      else
      {
        isBlankLine = false;
      }
      await writer.WriteLineAsync(l);

    }
    writer.Close();
  }

  private static readonly Icon IconCollapsed = new Icons.Regular.Size20.Folder();
  private static readonly Icon IconExpanded = new Icons.Regular.Size20.FolderOpen();
  private static readonly Icon IconFile = new Icons.Regular.Size20.Document();
  private static readonly string[] MdExtensions = [".md", ".mkd", ".mdwn", ".mdown", ".mdtxt", ".mdtext", ".markdown", ".text"];
  public async Task<List<ITreeViewItem>> GetDirectoryTreeAsync(string path)
  {
    var root = new DirectoryInfo(path);
    if (!root.Exists) throw new DirectoryNotFoundException();

    var children = new List<ITreeViewItem>();
    foreach (var folder in root.GetDirectories().OrderBy(x => x.Name))
    {
      var item = new TreeViewItem(folder.FullName, folder.Name);
      item.IconCollapsed = IconCollapsed;
      item.IconExpanded = IconExpanded;
      item.Items = await this.GetDirectoryTreeAsync(folder.FullName);
      children.Add(item);
    }
    foreach (var file in root.GetFiles().OrderBy(x => x.Name))
    {
      var item = new TreeViewItem(file.FullName, file.Name);
      item.IconCollapsed = IconFile;
      if (!MdExtensions.Contains(file.Extension.ToLower())) item.Disabled = true;
      children.Add(item);
    }
    return children;
  }
}

