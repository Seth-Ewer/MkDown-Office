using MkDownOffice.Contracts;

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
  public void SetCurrentFile(string filename)
  {
    this.CurrentFile = new MarkdownFile();
  }
}
