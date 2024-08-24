using MkDownOffice.Contracts;

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MkDownOffice.Services;

public class CabinetService : ICabinetService
{
  public static readonly string AppDataFolderName = "MkDownOffice";
  public static readonly string CabinetPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), AppDataFolderName);

  public List<string> GetCabinetNames()
  {
    return Directory.GetDirectories(CabinetPath)
                    .OrderBy(x => x)
                    .ToList();
  }
}


