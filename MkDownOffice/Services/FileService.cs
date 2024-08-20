using MkDownOffice.Contracts;
using MkDownOffice.Models;

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
    var reader = info.OpenText();
    mdFile.Markdown = await reader.ReadToEndAsync();

    return mdFile;
  }

  public async Task SaveFileAsync(MarkdownFile mdFile)
  {
    var info = new FileInfo(mdFile.Path);

    using var writer = info.CreateText();
    foreach (var l in mdFile.Markdown.Split('\n'))
    {
      await writer.WriteLineAsync(l);
    }
    writer.Close();
  }
}

