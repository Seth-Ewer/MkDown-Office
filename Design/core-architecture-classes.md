# Product Design

## Top 10 Features of this Application

These are the top user-level features or use cases for the application.

1. Edit Markdown files stored on your local computer.
2. Support all of the Markdown features in the same way that GitHub does.
   1. This is also called "GitHub Flavored Markdown" or GFM.
   2. Consider [GFM Spec](https://github.github.com/gfm/) for details.
3. It should be a sort-of WYSIWYG editor for Markdown files.
   1. Text and images should be displayed per GFM rules while editing.
   2. The editor should be able to display the rendering Markdown format codes on screen, but they should be rendered in a "grayed out" color to indicate they will not be visible to the end user.
   3. Note, we expect this to work more or less like it does when editing Markdown text in the Discord chat application.
4. Check those markdown files in/out from a Git repository.
5. Manage images to include in the Markdown files as a sub-folder under the file.
6. Build an index of Markdown files so we can show links and back-links in the UI.
7. Ensure that links between Markdown files allow navigation between them even after uploading to GitHub.

## Road-map

These are features that we would like to add in future releases but will not be included in v1 of the application.

1. [MARP](https://github.com/marp-team/marp) support to render presentations.
1. [MermaidJs](https://mermaid.js.org/) support for diagrams.
1. Task lists with check boxes that you can sign off.
1. Full support for all [GFM](https://github.github.com/gfm/) features.
1. [Liquid Formatting](https://docs.github.com/en/contributing/writing-for-github-docs/using-markdown-and-liquid-in-github-docs) in Markdown.


## Major Architecture Features of this Application

The application is intended to run on a local computer and interact with the local file system. It will also interact with a Git repository to check files in and out. The application will be built using the following technologies:

```mermaid
---
title: Major Architecture Features
---
classDiagram
class Photino_Window {
  string Title
  int Width
  int Height
  Facade Facade
  Show()
  Close()
}

class Photino_Services {
  FileService Files
  SearchService Search
  LinksService Links
  GitService Git
}
Photino_Window --> Photino_Services : Hosts Backend
ViewModel ..> Photino_Services : Dependency

class ViewModel {
  // Bindable Properties
  Folder RootFolder
  Folder CurrentFolder
  MarkdownFile CurrentFile  
  // Services
  SearchService _searchService
  LinksService _linksService
  GitService _gitService
  FileService _fileService
  SetRootFolder(path)
  SetCurrentFolder(path)
  SetCurrentFile(path)
  GetMarkdown()
}
Menu_UI ..> ViewModel : Inject
Editor_UI ..> ViewModel : Inject

class SearchService {
  MarkdownFile[] Search(string query)
}
Photino_Services --> SearchService : Uses

class LinksService {
  Links[] GetLinks(MarkdownFile file)
  Links[] GetBackLinks(MarkdownFile file)
}
Photino_Services --> LinksService : Uses

class GitService {
  void CheckIn(MarkdownFile file)
  void CheckOut(MarkdownFile file)
  void Pull();
  void Push();
}
Photino_Services --> GitService : Uses

class FileService {
  string ReadFile(string path)
  void WriteFile(MarkdownFile path, Markdown contents)
  string[] GetFiles(string path)
  string[] GetFolders(string path)
  void CreateFolder(string path)
  void CreateFile(string path)
  void DeleteFolder(string path)
  void DeleteFile(string path)
}
Photino_Services --> FileService : Uses
Folder ..> FileService : Photino_Services.Files

class Blazor_App {
  Menu_UI Menu
  Editor_UI Editor
  OpenFile(MarkdownFile file)
  SaveFile(MarkdownFile file, Markdown markdown)  
  ShowLinks(MarkdownFile file): Links[]
  ShowBackLinks(MarkdownFile file): Links[]
}
Photino_Window --> Blazor_App : Hosts Frontend

class Menu_UI {
  ViewModel ViewModel
}
Blazor_App --> Menu_UI : Shows

class Folder {
  string Name
  string Path
  string[] FolderNames
  MarkdownFile[] Files
  FileService FileService
  GetParentFolder(): Folder
  GetChildFolder(string name): Folder
}
ViewModel ..> Folder : RootFolder

class MarkdownFile {
  string Name
  string Path
  Markdown Markdown
}
Folder ..> MarkdownFile : Files
ViewModel ..> MarkdownFile : CurrentFile

class Editor_UI {
  string CurrentFile
  Markdown CurrentMarkdown
  RenderMarkdown()
}
Blazor_App --> Editor_UI : Shows

```










