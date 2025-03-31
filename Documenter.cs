using System;
using System.IO;
using System.Reflection;
using CrystalDocumenter.DocObjects;

namespace CrystalDocumenter;

partial class DocumenterProgram
{
  static void Main(string[] args)
  {
    HandleArguments(args);

    if (InputFile == string.Empty)
    {
      Console.WriteLine("No input provided!");
      Usage();
      return;
    }

    // Load your assembly here
    FileInfo assemblyPath = new(InputFile);
    DirectoryInfo outputDirInfo = new(OutputDir);

    if (!assemblyPath.Exists)
    {
      Console.WriteLine($"Unable to load assembly {assemblyPath.FullName}");
      return;
    }

    if (!outputDirInfo.Exists)
      outputDirInfo.Create();

    Assembly assembly = Assembly.LoadFrom(assemblyPath.FullName);
    AssemblyDocumentation doc = new(assembly);

    doc.CreateDocumentationSources(OutputDir);
    doc.BuildDocumentationPages(OutputDir);
    doc.BuildXMLDoc(OutputDir);
  }

}
