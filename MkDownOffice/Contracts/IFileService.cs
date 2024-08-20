using MkDownOffice.Models;

using System.Threading.Tasks;

namespace MkDownOffice.Contracts;

public interface IFileService
{
  public Task<Folder> OpenFolderAsync(string path);
  public Task<MarkdownFile> OpenFileAsync(string path);
  public Task SaveFileAsync(MarkdownFile mdFile);
}
