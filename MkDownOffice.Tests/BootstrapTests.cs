using System.Diagnostics.CodeAnalysis;

[assembly: ExcludeFromCodeCoverage]

namespace MkDownOffice.Tests
{

  public class BootstrapTests
  {
    [Fact]
    public void BootstrapTest()
    {
      Assert.True(true);
      Assert.False(false);
    }
  }
}