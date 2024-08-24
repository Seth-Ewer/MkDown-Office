using MkDownOffice.Contracts;
using MkDownOffice.Models;
using MkDownOffice.Services;

using Moq;

namespace MkDownOffice.Tests.Models;

public class ViewModelTests_RootFolder
{
  private readonly Mock<IFileService> _mockFileService;
  private readonly Mock<ILinkService> _mockLinkService;
  private readonly Mock<ISearchService> _mockSearchService;
  private readonly Mock<IGitService> _mockGitService;
  private readonly MarkdownViewModel _viewModel;
  private readonly string appDatapath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
  private readonly string unitTestFolderName = CabinetService.ApplicationDataFolderName + "_Test";
  private readonly string unitTestPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), CabinetService.ApplicationDataFolderName + "_Test");
  private readonly string unitTestProjectPath = Path.Combine(
    System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData),
    CabinetService.ApplicationDataFolderName + "_Test",
    CabinetService.ApplicationDataFolderName + "_Test");


  public ViewModelTests_RootFolder()
  {
    this._mockFileService = new Mock<IFileService>();
    this._mockLinkService = new Mock<ILinkService>();
    this._mockSearchService = new Mock<ISearchService>();
    this._mockGitService = new Mock<IGitService>();

    this._viewModel = new MarkdownViewModel(
        this._mockFileService.Object,
        this._mockLinkService.Object,
        this._mockSearchService.Object,
        this._mockGitService.Object
    );
  }

  ~ViewModelTests_RootFolder()
  {
    var path = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        this.unitTestFolderName
    );

    if (Directory.Exists(path))
    {
      Directory.Delete(path, true);
    }
  }

  [Fact]
  public void SetRootFolder_CreatesDirectoryIfNotExists()
  {
    // Arrange
    if (Directory.Exists(this.unitTestPath))
    {
      Directory.Delete(this.unitTestPath, true);
    }

    // Act
    this._viewModel.SetRootFolder(this.unitTestFolderName, true);

    // Assert
    Assert.True(Directory.Exists(this.unitTestPath));
  }

  [Fact]
  public void SetRootFolder_InitializesRootFolder()
  {
    // Arrange

    // Act
    this._viewModel.SetRootFolder(this.unitTestFolderName, true);

    // Assert
    Assert.NotNull(this._viewModel.RootFolder);
    Assert.Equal(this.unitTestProjectPath, this._viewModel.RootFolder.Path);
    Assert.Equal(this.unitTestFolderName, this._viewModel.RootFolder.Name);
  }

  [Fact]
  public void SetRootFolder_InitializesCurrentFolder()
  {
    // Arrange
    var path = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        this.unitTestFolderName
    );

    // Act
    this._viewModel.SetRootFolder(this.unitTestFolderName);

    // Assert
    Assert.NotNull(this._viewModel.CurrentFolder);
    Assert.Equal(this._viewModel.RootFolder, this._viewModel.CurrentFolder);
  }

  [Fact]
  public void CloseRootFolder_ShouldSetRootFolderAndCurrentFolderAndCurrentFileToNull()
  {
    // Arrange
    this._viewModel.SetRootFolder(this.unitTestFolderName, true);
    Assert.NotNull(this._viewModel.RootFolder);
    Assert.NotNull(this._viewModel.CurrentFolder);

    // Act
    this._viewModel.CloseRootFolder();

    // Assert
    Assert.Null(this._viewModel.RootFolder);
    Assert.Null(this._viewModel.CurrentFolder);
    Assert.Null(this._viewModel.CurrentFile);
  }
  [Fact]
  public void SetCurrentFolder_ShouldUpdateCurrentFolder()
  {
    // Arrange
    var name = "SubFolder";
    this._viewModel.SetRootFolder(this.unitTestFolderName);
    var path = Path.Combine(this._viewModel.CurrentFolder.Path, name);
    if (!Directory.Exists(path)) Directory.CreateDirectory(path);
    var initialFolder = this._viewModel.CurrentFolder;

    // Act
    this._viewModel.SetCurrentFolder(name);

    // Assert
    Assert.NotNull(this._viewModel.CurrentFolder);
    Assert.NotEqual(initialFolder, this._viewModel.CurrentFolder);
  }

  [Fact]
  public void SetCurrentFolderToParent_ShouldUpdateCurrentFolderToParent()
  {
    // Arrange
    var name = "SubFolder";
    this._viewModel.SetRootFolder(this.unitTestFolderName);
    var path = Path.Combine(this._viewModel.CurrentFolder.Path, name);
    if (!Directory.Exists(path)) Directory.CreateDirectory(path);
    this._viewModel.SetCurrentFolder(name);

    // Act
    this._viewModel.SetCurrentFolderToParent(1);

    // Assert
    Assert.NotNull(this._viewModel.CurrentFolder);
    Assert.Equal(this._viewModel.RootFolder.Path, this._viewModel.CurrentFolder.Path);
  }

  [Fact]
  public void GetBreadcrumbs_ShouldReturnCorrectBreadcrumbs()
  {
    // Arrange
    var name = "SubFolder";
    this._viewModel.SetRootFolder(this.unitTestFolderName);
    var path = Path.Combine(this._viewModel.CurrentFolder.Path, name);
    if (!Directory.Exists(path)) Directory.CreateDirectory(path);
    this._viewModel.SetCurrentFolder(name);

    // Act
    var breadcrumbs = this._viewModel.GetBreadcrumbsForCurrentFolder();

    // Assert
    var expectedBreadcrumbs = new List<string> { "SubFolder", this.unitTestFolderName };
    Assert.Equal(expectedBreadcrumbs, breadcrumbs);
  }
}
