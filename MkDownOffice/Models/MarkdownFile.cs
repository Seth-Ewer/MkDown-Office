namespace MkDownOffice.Models;

public class MarkdownFile
{
  private string _markdown { get; set; }

  public bool HasChanges { get; set; }
  public string Name { get; set; }
  public string Path { get; set; }
  public string Markdown
  {
    get => this._markdown;
    set
    {
      if (this._markdown == value) return;
      if (!string.IsNullOrEmpty(this._markdown))
        this.HasChanges = true;
      this._markdown = value;
    }
  }
}