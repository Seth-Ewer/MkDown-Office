namespace MkDownOffice.Models;

public class MarkdownFile
{
  private string _name { get; set; }
  private string _path { get; set; }
  private string _markdown { get; set; }

  public bool HasChanges { get; set; }
  public string Name { get => _name; set { _name = value; HasChanges = true; } }
  public string Path { get => _path; set { _path = value; HasChanges = true; } }
  public string Markdown { get => _markdown; set { _markdown = value; HasChanges = true; } }
}