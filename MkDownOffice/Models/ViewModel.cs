using MkDownOffice.Contracts;

using System.IO;
using System.Threading.Tasks;

namespace MkDownOffice.Models;

public class ViewModel
{
  private readonly IFileService _fileService;
  private readonly ILinkService _linkService;
  private readonly ISearchService _searchServie;
  private readonly IGitService _gitService;


  public ViewModel(
    IFileService fileService,
    ILinkService linkService,
    ISearchService searchService,
    IGitService gitService)
  {
    _fileService = fileService;
    _linkService = linkService;
    _searchServie = searchService;
    _gitService = gitService;
  }

  public Folder RootFolder { get; set; }
  public Folder CurrentFolder { get; set; }
  public MarkdownFile CurrentFile { get; set; }

  public void SetRootFolder(string path)
  {
    this.RootFolder = new Folder();
    this.CurrentFolder = this.RootFolder;
  }
  public void CloseRootFolder()
  {
    this.RootFolder = null;
    this.CurrentFolder = null;
    this.CurrentFile = null;
  }
  public void SetCurrentFolder(string path)
  {
    this.CurrentFolder = new Folder();
  }
  public async Task SetCurrentFile(string filename)
  {
    if (this.CurrentFile != null) this.Save();

    var info = new FileInfo(filename);
    if (!info.Exists) throw new FileNotFoundException();

    this.CurrentFile = new MarkdownFile();
    this.CurrentFile.Name = info.Name;
    this.CurrentFile.Path = info.FullName;
    var reader = info.OpenText();
    this.CurrentFile.Markdown = await reader.ReadToEndAsync();
  }

  public string GetRenderedMarkdownForCurrentFile()
  {
    return Markdig.Markdown.ToHtml(this.CurrentFile.Markdown);
  }

  public void Save()
  {
    var info = new FileInfo(this.CurrentFile.Path);
    if (!info.Exists) throw new FileNotFoundException();

    using var writer = info.CreateText();
    writer.WriteAsync(this.CurrentFile.Markdown);
    writer.Close();
  }
}
