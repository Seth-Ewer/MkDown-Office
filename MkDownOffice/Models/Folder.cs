using System.Collections.Generic;

namespace MkDownOffice.Models;

public class Folder
{
  public string Name { get; set; }
  public string Path { get; set; }
  public List<string> Folders { get; set; }
  public List<string> Files { get; set; }
}
