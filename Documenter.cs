/*
Copyright (C) 2025 Robyn (robyn@mamallama.dev)

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
*/

using System;
using System.IO;
using System.Reflection;
using CrystalDocumenter.DocObjects;

namespace CrystalDocumenter;

partial class DocumenterProgram
{
  public static string LICENSE => """
  ---------------------------------------------------------------------
  CrystalDocumenter Copyright (C) 2025  Robyn <Robyn@mamallama.dev>
    This program comes with ABSOLUTELY NO WARRANTY;
    This is free software, and you are welcome to redistribute it
    under certain conditions; See the included license for details.

  You should have received a copy of the GNU LesserGeneral Public License
  along with this program.  If not, see <https://www.gnu.org/licenses/>
  ---------------------------------------------------------------------
  
  """;
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
