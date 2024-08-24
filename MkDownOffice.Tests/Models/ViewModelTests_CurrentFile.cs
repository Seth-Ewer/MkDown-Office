using MkDownOffice.Contracts;
using MkDownOffice.Models;
using MkDownOffice.Services;

using Moq;

namespace MkDownOffice.Tests.Models;

public class ViewModelTests_CurrentFile
{
  private readonly Mock<IFileService> _mockFileService;
  private readonly Mock<ILinkService> _mockLinkService;
  private readonly Mock<ISearchService> _mockSearchService;
  private readonly Mock<IGitService> _mockGitService;
  private readonly MarkdownViewModel _viewModel;
  private readonly string appDatapath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
  private readonly string unitTestFolderName = CabinetService.AppDataFolderName + "_Test";
  private readonly string unitTestPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), CabinetService.AppDataFolderName + "_Test");

  public ViewModelTests_CurrentFile()
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

  [Fact]
  public async Task SetCurrentFile_ShouldThrowExceptionIfFolderNotOpen()
  {
    // Arrange
    var filename = "test.md";

    // Act & Assert
    await Assert.ThrowsAsync<DirectoryNotFoundException>(() => this._viewModel.SetCurrentFile(filename));
  }

  [Fact]
  public async Task SetCurrentFile_ShouldSaveCurrentFileIfHasChanges()
  {
    // Arrange
    var filename = "test.md";
    var currentFile = new MarkdownFile { HasChanges = true };
    this._viewModel.CurrentFile = currentFile;
    this._viewModel.RootFolder = new Folder { Path = this.unitTestPath };
    this._viewModel.CurrentFolder = new Folder { Path = this.unitTestPath };

    this._mockFileService.Setup(fs => fs.OpenFileAsync(It.IsAny<string>())).ReturnsAsync(new MarkdownFile());
    this._mockFileService.Setup(fs => fs.SaveFileAsync(It.IsAny<MarkdownFile>())).Returns(Task.CompletedTask);

    // Act
    await this._viewModel.SetCurrentFile(filename);

    // Assert
    this._mockFileService.Verify(fs => fs.SaveFileAsync(currentFile), Times.Once);
  }

  [Fact]
  public async Task SetCurrentFile_ShouldOpenNewFile()
  {
    // Arrange
    var filename = "test.md";
    var newFile = new MarkdownFile();
    this._viewModel.RootFolder = new Folder { Path = this.unitTestPath };
    this._viewModel.CurrentFolder = new Folder { Path = this.unitTestPath };

    this._mockFileService.Setup(fs => fs.OpenFileAsync(It.IsAny<string>())).ReturnsAsync(newFile);

    // Act
    await this._viewModel.SetCurrentFile(filename);

    // Assert
    Assert.Equal(newFile, this._viewModel.CurrentFile);
    this._mockFileService.Verify(fs => fs.OpenFileAsync(It.IsAny<string>()), Times.Once);
  }

  [Fact]
  public void GetRenderedMarkdownForCurrentFile_ShouldReturnHtml()
  {
    // Arrange
    var markdownContent = "# Hello World";
    var expectedHtml = "<h1>Hello World</h1>\n";
    this._viewModel.CurrentFile = new MarkdownFile { Markdown = markdownContent };

    // Act
    var result = this._viewModel.GetRenderedMarkdownForCurrentFile();

    // Assert
    Assert.Equal(expectedHtml, result);
  }
}
