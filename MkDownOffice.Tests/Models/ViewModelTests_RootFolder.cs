using MkDownOffice.Contracts;
using MkDownOffice.Models;

using Moq;

namespace MkDownOffice.Tests.Models;

public class ViewModelTests_RootFolder
{
  private readonly Mock<IFileService> _mockFileService;
  private readonly Mock<ILinkService> _mockLinkService;
  private readonly Mock<ISearchService> _mockSearchService;
  private readonly Mock<IGitService> _mockGitService;
  private readonly ViewModel _viewModel;
  const string unitTestFolderName = "MkDownOfficeUnitTestFolder";

  public ViewModelTests_RootFolder()
  {
    _mockFileService = new Mock<IFileService>();
    _mockLinkService = new Mock<ILinkService>();
    _mockSearchService = new Mock<ISearchService>();
    _mockGitService = new Mock<IGitService>();

    _viewModel = new ViewModel(
        _mockFileService.Object,
        _mockLinkService.Object,
        _mockSearchService.Object,
        _mockGitService.Object
    );
  }

  ~ViewModelTests_RootFolder()
  {
    var path = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        unitTestFolderName
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
    var path = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        unitTestFolderName
    );

    if (Directory.Exists(path))
    {
      Directory.Delete(path, true);
    }

    // Act
    _viewModel.SetRootFolder(unitTestFolderName);

    // Assert
    Assert.True(Directory.Exists(path));
  }

  [Fact]
  public void SetRootFolder_InitializesRootFolder()
  {
    // Arrange
    var path = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        unitTestFolderName
    );

    // Act
    _viewModel.SetRootFolder(unitTestFolderName);

    // Assert
    Assert.NotNull(_viewModel.RootFolder);
    Assert.Equal(path, _viewModel.RootFolder.Path);
    Assert.Equal(unitTestFolderName, _viewModel.RootFolder.Name);
  }

  [Fact]
  public void SetRootFolder_InitializesCurrentFolder()
  {
    // Arrange
    var path = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        unitTestFolderName
    );

    // Act
    _viewModel.SetRootFolder(unitTestFolderName);

    // Assert
    Assert.NotNull(_viewModel.CurrentFolder);
    Assert.Equal(_viewModel.RootFolder, _viewModel.CurrentFolder);
  }

  [Fact]
  public void CloseRootFolder_ShouldSetRootFolderAndCurrentFolderAndCurrentFileToNull()
  {
    // Arrange
    _viewModel.SetRootFolder(unitTestFolderName);
    Assert.NotNull(_viewModel.RootFolder);
    Assert.NotNull(_viewModel.CurrentFolder);

    // Act
    _viewModel.CloseRootFolder();

    // Assert
    Assert.Null(_viewModel.RootFolder);
    Assert.Null(_viewModel.CurrentFolder);
    Assert.Null(_viewModel.CurrentFile);
  }
  [Fact]
  public void SetCurrentFolder_ShouldUpdateCurrentFolder()
  {
    // Arrange
    var name = "SubFolder";
    _viewModel.SetRootFolder(unitTestFolderName);
    var path = Path.Combine(_viewModel.CurrentFolder.Path, name);
    if (!Directory.Exists(path)) Directory.CreateDirectory(path);
    var initialFolder = _viewModel.CurrentFolder;

    // Act
    _viewModel.SetCurrentFolder(name);

    // Assert
    Assert.NotNull(_viewModel.CurrentFolder);
    Assert.NotEqual(initialFolder, _viewModel.CurrentFolder);
  }

  [Fact]
  public void SetCurrentFolderToParent_ShouldUpdateCurrentFolderToParent()
  {
    // Arrange
    var name = "SubFolder";
    _viewModel.SetRootFolder(unitTestFolderName);
    var path = Path.Combine(_viewModel.CurrentFolder.Path, name);
    if (!Directory.Exists(path)) Directory.CreateDirectory(path);
    _viewModel.SetCurrentFolder(name);

    // Act
    _viewModel.SetCurrentFolderToParent(1);

    // Assert
    Assert.NotNull(_viewModel.CurrentFolder);
    Assert.Equal(_viewModel.RootFolder.Path, _viewModel.CurrentFolder.Path);
  }

  [Fact]
  public void GetBreadcrumbs_ShouldReturnCorrectBreadcrumbs()
  {
    // Arrange
    var name = "SubFolder";
    _viewModel.SetRootFolder(unitTestFolderName);
    var path = Path.Combine(_viewModel.CurrentFolder.Path, name);
    if (!Directory.Exists(path)) Directory.CreateDirectory(path);
    _viewModel.SetCurrentFolder(name);

    // Act
    var breadcrumbs = _viewModel.GetBreadcrumbs();

    // Assert
    var expectedBreadcrumbs = new List<string> { "SubFolder", unitTestFolderName };
    Assert.Equal(expectedBreadcrumbs, breadcrumbs);
  }
}

public class ViewModelTests_CurrentFile
{
  private readonly Mock<IFileService> _mockFileService;
  private readonly Mock<ILinkService> _mockLinkService;
  private readonly Mock<ISearchService> _mockSearchService;
  private readonly Mock<IGitService> _mockGitService;
  private readonly ViewModel _viewModel;
  const string unitTestFolderName = "MkDownOfficeUnitTestFolder";

  public ViewModelTests_CurrentFile()
  {
    _mockFileService = new Mock<IFileService>();
    _mockLinkService = new Mock<ILinkService>();
    _mockSearchService = new Mock<ISearchService>();
    _mockGitService = new Mock<IGitService>();

    _viewModel = new ViewModel(
        _mockFileService.Object,
        _mockLinkService.Object,
        _mockSearchService.Object,
        _mockGitService.Object
    );
  }

  [Fact]
  public async Task SetCurrentFile_ShouldThrowExceptionIfFolderNotOpen()
  {
    // Arrange
    var filename = "test.md";

    // Act & Assert
    await Assert.ThrowsAsync<DirectoryNotFoundException>(() => _viewModel.SetCurrentFile(filename));
  }

  [Fact]
  public async Task SetCurrentFile_ShouldSaveCurrentFileIfHasChanges()
  {
    // Arrange
    var filename = "test.md";
    var currentFile = new MarkdownFile { HasChanges = true };
    _viewModel.CurrentFile = currentFile;
    _viewModel.RootFolder = new Folder { Path = "root" };
    _viewModel.CurrentFolder = new Folder { Path = "root" };

    _mockFileService.Setup(fs => fs.OpenFileAsync(It.IsAny<string>())).ReturnsAsync(new MarkdownFile());
    _mockFileService.Setup(fs => fs.SaveFileAsync(It.IsAny<MarkdownFile>())).Returns(Task.CompletedTask);

    // Act
    await _viewModel.SetCurrentFile(filename);

    // Assert
    _mockFileService.Verify(fs => fs.SaveFileAsync(currentFile), Times.Once);
  }

  [Fact]
  public async Task SetCurrentFile_ShouldOpenNewFile()
  {
    // Arrange
    var filename = "test.md";
    var newFile = new MarkdownFile();
    _viewModel.RootFolder = new Folder { Path = "root" };
    _viewModel.CurrentFolder = new Folder { Path = "root" };

    _mockFileService.Setup(fs => fs.OpenFileAsync(It.IsAny<string>())).ReturnsAsync(newFile);

    // Act
    await _viewModel.SetCurrentFile(filename);

    // Assert
    Assert.Equal(newFile, _viewModel.CurrentFile);
    _mockFileService.Verify(fs => fs.OpenFileAsync(It.IsAny<string>()), Times.Once);
  }

  [Fact]
  public void GetRenderedMarkdownForCurrentFile_ShouldReturnHtml()
  {
    // Arrange
    var markdownContent = "# Hello World";
    var expectedHtml = "<h1>Hello World</h1>\n";
    _viewModel.CurrentFile = new MarkdownFile { Markdown = markdownContent };

    // Act
    var result = _viewModel.GetRenderedMarkdownForCurrentFile();

    // Assert
    Assert.Equal(expectedHtml, result);
  }
}
