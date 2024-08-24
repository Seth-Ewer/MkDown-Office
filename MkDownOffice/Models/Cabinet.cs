using System;

namespace MkDownOffice.Models;

public class Cabinet
{
  public string Name { get; set; }
  public string Path { get; set; }
  public DateTime Created { get; set; }
  public DateTime Modified { get; set; }
}