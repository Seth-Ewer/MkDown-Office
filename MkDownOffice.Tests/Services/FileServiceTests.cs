using Microsoft.VisualStudio.TestPlatform.PlatformAbstractions;

using MkDownOffice.Models;
using MkDownOffice.Services;

using System.Reflection;

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

  [Fact]
  public async Task OpenFileAsync_ExistingFile_ReturnsMarkdownFile()
  {
    // Arrange
    var filePath = Assembly.GetExecutingAssembly().GetAssemblyLocation();
    filePath = filePath.Substring(0, filePath.LastIndexOf("\\"));
    filePath = Path.Combine(filePath, "TestData", "OnlyMarkdownFiles", "SampleFileOne.md");


    // Act
    var SUT = new FileService();
    var result = await SUT.OpenFileAsync(filePath);

    // Assert
    Assert.NotNull(result);
    Assert.Equal("SampleFileOne.md", result.Name);
    Assert.Equal(filePath, result.Path);
    Assert.StartsWith("# Sample File", result.Markdown);
  }

  [Fact]
  public async Task OpenFileAsync_NonExistingFile_ThrowsFileNotFoundException()
  {
    // Arrange
    var filePath = "path/to/nonexisting/file.md";


    // Act and Assert
    var SUT = new FileService();
    await Assert.ThrowsAsync<FileNotFoundException>(() => SUT.OpenFileAsync(filePath));
  }

  [Fact]
  public async Task SaveFileAsync_ShouldWriteMarkdownToFile()
  {
    // Arrange
    var mdFile = new MarkdownFile
    {
      Name = "test.md",
      Path = Path.Combine("test.md"),
      Markdown = "This is a test markdown file."
    };

    if (File.Exists(mdFile.Path)) File.Delete(mdFile.Path);

    // Act
    var SUT = new FileService();
    await SUT.SaveFileAsync(mdFile);

    // Assert
    Assert.True(File.Exists(mdFile.Path));
  }
}
