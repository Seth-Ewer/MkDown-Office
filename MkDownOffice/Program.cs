using Microsoft.Extensions.DependencyInjection;

using MkDownOffice.Contracts;
using MkDownOffice.Models;
using MkDownOffice.Services;

using Photino.Blazor;
using Photino.NET;
using System;

namespace MkDownOffice
{
  class Program
  {

    public static PhotinoWindow MainWindow { get; private set; }

    [STAThread]
    static void Main(string[] args)
    {
      var appBuilder = PhotinoBlazorAppBuilder.CreateDefault(args);

      appBuilder.Services.AddLogging();

      // Register Core Application Services
      appBuilder.Services.AddSingleton<IFileService, FileService>();
      appBuilder.Services.AddSingleton<ILinkService, LinkService>();
      appBuilder.Services.AddSingleton<ISearchService, SearchService>();
      appBuilder.Services.AddSingleton<IGitService, GitService>();

      appBuilder.Services.AddSingleton<ViewModel>();

      // register root component and selector
      appBuilder.RootComponents.Add<App>("app");

      var app = appBuilder.Build();

      // customize window
      app.MainWindow
          .SetIconFile("favicon.ico")
          .SetTitle("MkDown Office")
          .SetDevToolsEnabled(true);
      
      Program.MainWindow = app.MainWindow;

      AppDomain.CurrentDomain.UnhandledException += (sender, error) =>
      {
        app.MainWindow.ShowMessage("Fatal exception", error.ExceptionObject.ToString());
      };

      app.Run();
    }
  }
}
