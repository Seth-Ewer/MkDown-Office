using Microsoft.FluentUI.AspNetCore.Components;

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
    this._fileService = fileService;
    this._linkService = linkService;
    this._searchService = searchService;
    this._gitService = gitService;
  }

  public long WindowHeight
  {
    get
    {
      long calc = 300;
      try
      {
        calc = (Program.MainWindow.Height       // full height of window
               / Program.MainWindow.ScreenDpi    // divided by screen dpi to find inches
               * 96)                             // times 90 to get browser scale pixel count
               - Program.MainWindow.ScreenDpi;   // subtract 1/2 inch height of the title bar
      }
      catch {/* doesn't matter */}
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
    get => this._rootFolder;
    set => this.SetValue(ref this._rootFolder, value);
  }

  private Folder _currentFolder;
  public Folder CurrentFolder
  {
    get => this._currentFolder;
    set => this.SetValue(ref this._currentFolder, value);
  }

  private MarkdownFile _currentFile;
  public MarkdownFile CurrentFile
  {
    get => this._currentFile;
    set => this.SetValue(ref this._currentFile, value);
  }

  private string _currentMenu;
  public string CurrentMenu
  {
    get => this._currentMenu;
    set => this.SetValue(ref this._currentMenu, value);
  }

  public void SetRootFolder(string rootFolderName = "MkDownOffice", string projectName = "Default")
  {
    try
    {
      var path = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        rootFolderName
        );

      if (!Directory.Exists(path))
        Directory.CreateDirectory(path);

      path = Path.Combine(path, projectName);

      if (!Directory.Exists(path))
        Directory.CreateDirectory(path);


      var tempFolder = new Folder();
      tempFolder.Path = path;

      var dirInfo = new DirectoryInfo(path);

      tempFolder.Name = dirInfo.Name;
      tempFolder.Folders = (from dir in dirInfo.GetDirectories() select dir.Name).ToList();
      tempFolder.Files = (from file in dirInfo.GetFiles() select file.Name).ToList();

      this.RootFolder = tempFolder;

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
    var path = Path.Combine(this.CurrentFolder.Path, name);
    var tempFolder = new Folder();
    tempFolder.Path = path;

    var dirInfo = new DirectoryInfo(path);

    tempFolder.Name = dirInfo.Name;
    tempFolder.Folders = (from dir in dirInfo.GetDirectories() select dir.Name).ToList();
    tempFolder.Files = (from file in dirInfo.GetFiles() select file.Name).ToList();

    this.CurrentFolder = tempFolder;
  }

  public void SetCurrentFolderToParent(int steps)
  {
    var dirInfo = new DirectoryInfo(this.CurrentFolder.Path);
    for (var i = 0; i < steps; i++)
    {
      dirInfo = dirInfo.Parent;
    }
    var tempFolder = new Folder();

    tempFolder.Path = dirInfo.FullName;
    tempFolder.Name = dirInfo.Name;
    tempFolder.Folders = (from dir in dirInfo.GetDirectories() select dir.Name).ToList();
    tempFolder.Files = (from file in dirInfo.GetFiles() select file.Name).ToList();

    this.CurrentFolder = tempFolder;
  }

  public async Task<List<ITreeViewItem>> GetDirectoryTreeAsync(string path = "")
  {
    if (this.RootFolder == null) return [];
    if (string.IsNullOrEmpty(path))
    {
      path = this.RootFolder.Path;
    }
    var found = await this._fileService.GetDirectoryTreeAsync(path);
    if (this.CurrentFolder != null)
      found = this.ExpandTree(this.CurrentFolder.Path, found);

    return found;
  }

  private List<ITreeViewItem> ExpandTree(string path, List<ITreeViewItem> tree)
  {
    foreach (var item in tree)
    {
      if (path.StartsWith(item.Id))
      {
        item.Expanded = true;
        if (item.Items.Any())
        {
          item.Items = this.ExpandTree(path, item.Items.ToList());
        }
      }
    }
    return tree;
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
      await this.SaveAsync();

    this.CurrentFile = await this._fileService.OpenFileAsync(path);
  }

  public async Task SaveAsync()
  {
    if (this.CurrentFile.HasChanges) await this._fileService.SaveFileAsync(this.CurrentFile);
    this.CurrentFile.HasChanges = false;
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
    this.OnPropertyChanged(propertyName);
  }
  #endregion INotifyPropertyChanged
}
