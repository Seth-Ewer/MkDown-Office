using MkDownOffice.Services;

namespace MkDownOffice.Tests.Services;

public class FileServiceTests
{
  [Fact]
  public async Task WhenFileService_OpensFolderWithOnlyFiles_RootFolderShouldHaveFilesButNoSubFolders()
  {
    // Arrange
    var path = Path.Combine(Directory.GetCurrentDirectory(), "TestData", "OnlyMarkdownFiles");

    // Act
    var SUT = new FileService();
    var folder = await SUT.OpenFolderAsync(path);

    // Assert
    Assert.Empty(folder.Folders);
    Assert.NotEmpty(folder.Files);

    Assert.Equal(3, folder.Files.Count);

    Assert.Equal("SampleFileOne.md", folder.Files[0]);
    Assert.Equal("SampleFileThree.md", folder.Files[1]);
    Assert.Equal("SampleFileTwo.md", folder.Files[2]);

  }

  [Fact]
  public async Task WhenFileService_OpensFolderWithFolders_RootFolderShouldHaveChildFoldersButNoFiles()
  {
    // Arrange
    var path = Path.Combine(Directory.GetCurrentDirectory(), "TestData", "OnlyChildFolders");

    // Act
    var SUT = new FileService();
    var folder = await SUT.OpenFolderAsync(path);

    // Assert
    Assert.Empty(folder.Files);
    Assert.NotEmpty(folder.Folders);

    Assert.Equal(3, folder.Folders.Count);

    Assert.Equal("One", folder.Folders[0]);
    Assert.Equal("Three", folder.Folders[1]);
    Assert.Equal("Two", folder.Folders[2]);

  }

  [Fact]
  public async Task WhenFileService_OpensFolderWithFilesAndFolders_RootFolderShouldHaveChildren()
  {
    // Arrange
    var path = Path.Combine(Directory.GetCurrentDirectory(), "TestData", "MarkdownFilesAndChildFolders");

    // Act
    var SUT = new FileService();
    var folder = await SUT.OpenFolderAsync(path);

    // Assert
    Assert.NotEmpty(folder.Files);
    Assert.NotEmpty(folder.Folders);

    Assert.Equal(3, folder.Files.Count);
    Assert.Equal(3, folder.Folders.Count);

    Assert.Equal("One", folder.Folders[0]);
    Assert.Equal("Three", folder.Folders[1]);
    Assert.Equal("Two", folder.Folders[2]);

    Assert.Equal("SampleFileOne.md", folder.Files[0]);
    Assert.Equal("SampleFileThree.md", folder.Files[1]);
    Assert.Equal("SampleFileTwo.md", folder.Files[2]);
  }

  [Fact]
  public async Task WhenFileService_OpensEmptyFolder_RootFolderShouldHaveNoChildren()
  {
    // Arrange
    var path = Path.Combine(Directory.GetCurrentDirectory(), "EmptyFolder");
    if (!Directory.Exists(path)) Directory.CreateDirectory(path);


    // Act
    var SUT = new FileService();
    var folder = await SUT.OpenFolderAsync(path);

    // Assert
    Assert.Empty(folder.Folders);
    Assert.Empty(folder.Files);

  }

  [Fact]
  public async Task WhenFileService_OpensNonexistentFolder_ShouldThrowException()
  {
    // Arrange

    // Act
    var SUT = new FileService();

    // Assert

    await Assert.ThrowsAsync<DirectoryNotFoundException>(async () => await SUT.OpenFolderAsync("NonexistentFolder"));

  }
}
