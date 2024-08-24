using MkDownOffice.Contracts;
using MkDownOffice.Models;

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MkDownOffice.Services;

public class CabinetService : ICabinetService
{
  public static readonly string ApplicationDataFolderName = "MkDownOffice";
  public static readonly string CabinetPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), ApplicationDataFolderName);

  public List<Cabinet> GetCabinetNames()
  {
    var root = new DirectoryInfo(CabinetPath);
    var found = root.GetDirectories()
                    .Where(x => !x.Name.StartsWith('.'))
                    .OrderBy(x => x.Name)
                    .Select(x => new Cabinet()
                    {
                      Name = x.Name,
                      Path = x.FullName,
                      Created = x.CreationTime,
                      Modified = x.LastWriteTime
                    })
                    .ToList();
    return found;
  }
}


