using MkDownOffice.Contracts;
using MkDownOffice.Models;
using MkDownOffice.Services;

using Moq;

namespace MkDownOffice.Tests.Models;

public class ViewModel_PropertyChangedTests
{
  private readonly Mock<IFileService> _mockFileService;
  private readonly Mock<ILinkService> _mockLinkService;
  private readonly Mock<ISearchService> _mockSearchService;
  private readonly Mock<IGitService> _mockGitService; private readonly MarkdownViewModel _viewModel;
  private readonly string appDatapath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
  private readonly string unitTestFolderName = CabinetService.ApplicationDataFolderName + "_Test";
  private readonly string unitTestPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), CabinetService.ApplicationDataFolderName + "_Test");


  public ViewModel_PropertyChangedTests()
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
  public void SetValue_ShouldSetBackingFieldAndRaisePropertyChanged()
  {
    // Arrange
    bool rootChanged = false;
    bool currentChanged = false;

    this._viewModel.PropertyChanged += (sender, args) =>
    {
      if (args.PropertyName == nameof(this._viewModel.RootFolder))
      {
        rootChanged = true;
      }
      if (args.PropertyName == nameof(this._viewModel.CurrentFolder))
      {
        currentChanged = true;
      }
    };

    // Act
    this._viewModel.SetRootFolder(this.unitTestFolderName, true);

    // Assert
    Assert.True(rootChanged);
    Assert.True(currentChanged);
  }
}