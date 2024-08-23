using Microsoft.FluentUI.AspNetCore.Components;

using MkDownOffice.Models;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace MkDownOffice.Contracts;

public interface IFileService
{
  public Task<Folder> OpenFolderAsync(string path);
  public Task<MarkdownFile> OpenFileAsync(string path);
  public Task SaveFileAsync(MarkdownFile mdFile);
  Task<List<ITreeViewItem>> GetDirectoryTreeAsync(string path);
}
