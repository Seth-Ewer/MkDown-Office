using MkDownOffice.Models;

using System.Threading.Tasks;

namespace MkDownOffice.Contracts;

public interface IFileService
{
  public Task<Folder> OpenFolderAsync(string path);
}
