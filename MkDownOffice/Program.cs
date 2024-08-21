using Microsoft.Extensions.DependencyInjection;

using MkDownOffice.Contracts;
using MkDownOffice.Models;
using MkDownOffice.Services;

using Microsoft.FluentUI.AspNetCore.Components;

using Photino.Blazor;

using System;

namespace MkDownOffice
{
  class Program
  {
    [STAThread]
    static void Main(string[] args)
    {
      var appBuilder = PhotinoBlazorAppBuilder.CreateDefault(args);

      appBuilder.Services.AddLogging();
      appBuilder.Services.AddFluentUIComponents();

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

      AppDomain.CurrentDomain.UnhandledException += (sender, error) =>
      {
        app.MainWindow.ShowMessage("Fatal exception", error.ExceptionObject.ToString());
      };

      app.Run();
    }
  }
}
