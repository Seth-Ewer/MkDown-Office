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
}

