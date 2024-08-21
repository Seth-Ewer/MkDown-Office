using MkDownOffice.Models;
using MkDownOffice.Services;

namespace MkDownOffice.Tests.Models;

public class ViewModelFolderTests
{
  [Fact]
  public void ViewModel_SaysFolderNotOpen_WhenRootFolderIsNull()
  {
    // Arrange
    var SUT = new ViewModel(new FileService(), new LinkService(), new SearchService(), new GitService());
    var expectedValue = false;

    // Act
    var result = SUT.IsFolderOpen;

    // Assert
    Assert.Equal(expectedValue, result);
  }

  [Fact]
  public async Task ViewModel_SaysFolderIsOpen_AfterChoosingARootFolder()
  {
    // Arrange    
    var FS = new FileService();
    var SUT = new ViewModel(FS, new LinkService(), new SearchService(), new GitService());
    var expectedValue = true;

    SUT.SetRootFolder();
    var mdFile = new MarkdownFile
    {
      Name = "test.md",
      Path = Path.Combine(SUT.RootFolder.Path, "test.md"),
      Markdown = "# Heading\n\nThis is a test markdown file."
    };

    if (File.Exists(mdFile.Path)) File.Delete(mdFile.Path);
    await FS.SaveFileAsync(mdFile);

    // Act
    await SUT.SetCurrentFile(Path.Combine(SUT.RootFolder.Path, "test.md"));
    var result = SUT.IsFolderOpen;

    // Assert
    Assert.Equal(expectedValue, result);
  }
}
