using MkDownOffice.Contracts;

using Photino.NET;

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
  public PhotinoWindow _mainWindow => Program.MainWindow;

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

  public bool IsFolderOpen { get => this.RootFolder != null && this.CurrentFolder != null; }

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

  public void SetRootFolder()
  {
    try
    {

      var path = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "mkdownoffice"
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
  public async Task SetCurrentFile(string filename)
  {
    if (!this.IsFolderOpen) { throw new DirectoryNotFoundException(); }

    var path = Path.Combine(this.CurrentFolder.Path, filename);

    if (this.CurrentFile != null && this.CurrentFile.HasChanges)
      await _fileService.SaveFileAsync(this.CurrentFile);

    this.CurrentFile = await _fileService.OpenFileAsync(path);
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
  protected void SetValue<T>(ref T backingFiled, T value, [CallerMemberName] string propertyName = null)
  {
    if (EqualityComparer<T>.Default.Equals(backingFiled, value)) return; backingFiled = value;
    OnPropertyChanged(propertyName);
  }

  #endregion INotifyPropertyChanged
}
