using MkDownOffice.Contracts;
using MkDownOffice.Models;

using Moq;

namespace MkDownOffice.Tests.Models;

public class ViewModel_PropertyChangedTests
{
  private readonly Mock<IFileService> _mockFileService;
  private readonly Mock<ILinkService> _mockLinkService;
  private readonly Mock<ISearchService> _mockSearchService;
  private readonly Mock<IGitService> _mockGitService;
  private readonly ViewModel _viewModel;
  const string unitTestFolderName = "MkDownOfficeUnitTestFolder";

  public ViewModel_PropertyChangedTests()
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
  public void SetValue_ShouldSetBackingFieldAndRaisePropertyChanged()
  {
    // Arrange
    bool rootChanged = false;
    bool currentChanged = false;

    _viewModel.PropertyChanged += (sender, args) =>
    {
      if (args.PropertyName == nameof(_viewModel.RootFolder))
      {
        rootChanged = true;
      }
      if (args.PropertyName == nameof(_viewModel.CurrentFolder))
      {
        currentChanged = true;
      }
    };

    // Act
    _viewModel.SetRootFolder(unitTestFolderName);

    // Assert
    Assert.True(rootChanged);
    Assert.True(currentChanged);
  }
}