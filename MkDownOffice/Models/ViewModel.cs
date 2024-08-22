using MkDownOffice.Contracts;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace MkDownOffice.Models;

public class ViewModel : INotifyPropertyChanged
{
  private readonly IFileService _fileService;
  private readonly ILinkService _linkService;
  private readonly ISearchService _searchService;
  private readonly IGitService _gitService;

  public ViewModel(
    IFileService fileService,
    ILinkService linkService,
    ISearchService searchService,
    IGitService gitService)
  {
    _fileService = fileService;
    _linkService = linkService;
    _searchService = searchService;
    _gitService = gitService;
  }

  public long WindowHeight 
  { 
    get {
    long calc = 300;
    try{
      calc = ( Program.MainWindow.Height       // full height of window
             / Program.MainWindow.ScreenDpi    // divided by screen dpi to find inches
             * 96)                             // times 90 to get browser scale pixel count
             - Program.MainWindow.ScreenDpi;   // subtract 1/2 inch height of the title bar
    }
    catch{/* doesn't matter */}
    return calc;
    }
  }
  public bool IsFolderOpen
  {
    get => this.RootFolder != null && this.CurrentFolder != null;
  }

  private Folder _rootFolder;
  public Folder RootFolder
  {
    get => _rootFolder;
    set => SetValue(ref _rootFolder, value);
  }

  private Folder _currentFolder;
  public Folder CurrentFolder
  {
    get => _currentFolder;
    set => SetValue(ref _currentFolder, value);
  }

  private MarkdownFile _currentFile;
  public MarkdownFile CurrentFile
  {
    get => _currentFile;
    set => SetValue(ref _currentFile, value);
  }

  public void SetRootFolder(string rootFolderName = "MkDownOffice")
  {
    try
    {
      var path = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        rootFolderName
        );

      if (!Directory.Exists(path))
        Directory.CreateDirectory(path);

      this.RootFolder = new Folder();
      this.RootFolder.Path = path;

      var dirInfo = new DirectoryInfo(path);

      this.RootFolder.Name = dirInfo.Name;
      this.RootFolder.Folders = (from dir in dirInfo.GetDirectories() select dir.Name).ToList();
      this.RootFolder.Files = (from file in dirInfo.GetFiles() select file.Name).ToList();

      this.CurrentFolder = this.RootFolder;

    }
    catch (Exception ex)
    {
      var temp = ex;
    }
  }

  public void CloseRootFolder()
  {
    this.RootFolder = null;
    this.CurrentFolder = null;
    this.CurrentFile = null;
  }

  public void SetCurrentFolder(string name)
  {
    var path = Path.Combine(CurrentFolder.Path, name);
    this.CurrentFolder = new Folder();
    this.CurrentFolder.Path = path;

    var dirInfo = new DirectoryInfo(path);

    this.CurrentFolder.Name = dirInfo.Name;
    this.CurrentFolder.Folders = (from dir in dirInfo.GetDirectories() select dir.Name).ToList();
    this.CurrentFolder.Files = (from file in dirInfo.GetFiles() select file.Name).ToList();
  }

  public void SetCurrentFolderToParent(int steps)
  {
    var dirInfo = new DirectoryInfo(this.CurrentFolder.Path);
    for (var i = 0; i < steps; i++)
    {
      dirInfo = dirInfo.Parent;
    }
    this.CurrentFolder = new Folder();

    this.CurrentFolder.Path = dirInfo.FullName;
    this.CurrentFolder.Name = dirInfo.Name;
    this.CurrentFolder.Folders = (from dir in dirInfo.GetDirectories() select dir.Name).ToList();
    this.CurrentFolder.Files = (from file in dirInfo.GetFiles() select file.Name).ToList();
  }

  public List<string> GetBreadcrumbs()
  {
    var Crumbs = new List<string>();
    var dirInfo = new DirectoryInfo(this.CurrentFolder.Path);
    while (dirInfo.Name != this.RootFolder.Name)
    {
      Crumbs.Add(dirInfo.Name);
      dirInfo = dirInfo.Parent;
    }
    Crumbs.Add(this.RootFolder.Name);
    return Crumbs;
  }

  public async Task SetCurrentFile(string filename)
  {
    if (!this.IsFolderOpen) { throw new DirectoryNotFoundException(); }

    var path = Path.Combine(this.CurrentFolder.Path, filename);

    if (this.CurrentFile != null && this.CurrentFile.HasChanges)
      await this.Save();

    this.CurrentFile = await _fileService.OpenFileAsync(path);
  }

  public async Task Save()
  {
    await _fileService.SaveFileAsync(this.CurrentFile);
  }

  public string GetRenderedMarkdownForCurrentFile()
  {
    return Markdig.Markdown.ToHtml(this.CurrentFile.Markdown);
  }

  #region INotifyPropertyChanged
  public event PropertyChangedEventHandler PropertyChanged;
  protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
  {
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }
  protected void SetValue<T>(ref T backingField, T value, [CallerMemberName] string propertyName = null)
  {
      if (EqualityComparer<T>.Default.Equals(backingField, value)) return;
      backingField = value;
      OnPropertyChanged(propertyName);
  }
  #endregion INotifyPropertyChanged
}
