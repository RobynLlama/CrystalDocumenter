using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json;
using CrystalDocumenter.Utilities;
using Markdig;

namespace CrystalDocumenter.DocObjects;

public class AssemblyDocumentation
{
  public readonly Assembly Origin;
  protected readonly Dictionary<string, TypeDocumentation> TypeInfos = [];
  public TypeDocumentation GetOrAddType(Type thisType)
  {
    if (TypeInfos.TryGetValue(Utils.HashType(thisType), out var info))
      return info;

    var newInfo = new TypeDocumentation(this, thisType);
    return TypeInfos[newInfo.InfoHash] = newInfo;
  }

  public AssemblyDocumentation(Assembly from)
  {
    Origin = from;
  }

  public AssemblyDocumentation(string fname)
  {
    FileInfo item = new(fname);
    if (!item.Exists)
      throw new Exception($"File not found: {item.FullName}");

    Origin = Assembly.LoadFrom(item.FullName);
  }

  public bool TryGetTypeFromHash(string hash, [NotNullWhen(true)] out TypeDocumentation? value)
  {
    if (TypeInfos.TryGetValue(hash, out var thing))
    {
      value = thing;
      return true;
    }

    value = null;
    return false;
  }

  public void CreateDocumentationSources(string outputDir = "Documentation")
  {
    TypeInfos.Clear();
    DirectoryInfo intermediary = new(Path.Combine(outputDir, "intermediary"));
    FileInfo indexFile = new(Path.Combine(outputDir, "IntermediaryIndex.md"));
    using StreamWriter index = new(indexFile.FullName);
    index.WriteLine($"# {Origin.GetName().Name}\n");

    if (!intermediary.Exists)
      intermediary.Create();

    var options = new JsonSerializerOptions() { WriteIndented = true };

    foreach (Type type in Origin.GetTypes())
    {
      var thing = GetOrAddType(type);
      Console.WriteLine($"Added: {thing.Name}");

      //Don't write nested classes in the index right now
      if (thing.NestedIn is null)
        index.WriteLine($"- [{thing.Name}](./intermediary/{thing.FullName}.json)");
    }

    foreach (var item in TypeInfos)
    {
      FileInfo output = new(Path.Combine(intermediary.FullName, $"{item.Value.FullName}.json"));

      if (output.Exists)
      {
        Console.WriteLine($"Skipping output for {item.Value.Name}");
        continue;
      }

      using StreamWriter writer = new(output.FullName);
      writer.Write(JsonSerializer.Serialize(item.Value, options));
    }
  }

  public void BuildDocumentationPages(string inputLocation = "Documentation")
  {
    TypeInfos.Clear();
    DirectoryInfo input = new(Path.Combine(inputLocation, "intermediary"));
    DirectoryInfo output = new(Path.Combine(inputLocation, "pages"));

    if (!output.Exists)
      output.Create();

    if (!input.Exists)
      throw new Exception($"Input directory does not exist {input.FullName}");

    var files = input.GetFiles("*.json");

    foreach (var file in files)
    {
      using StreamReader reader = new(file.FullName);
      var jsonText = reader.ReadToEnd();

      if (JsonSerializer.Deserialize<TypeDocumentation>(jsonText) is not TypeDocumentation item)
        throw new Exception($"Unable to deserialize a TypeDocumentation from {file.FullName}");

      TypeInfos[item.InfoHash] = item;
    }

    using TextWriter indexFile = new StreamWriter(Path.Combine(inputLocation, "Index.html"));
    StringBuilder index = new();

    index.Append(Utils.DefaultHeader);
    index.AppendLine($"# {Origin.GetName().Name}\n");

    foreach (var doc in TypeInfos)
    {
      var location = Path.Combine(output.FullName, $"{doc.Value.FullName}.html");
      using TextWriter writer = new StreamWriter(location);
      StringBuilder data = new(Utils.DefaultHeader);

      doc.Value.Save(data);
      data.Append(Utils.DefaultFooter);
      Markdown.ToHtml(data.ToString(), writer);

      //Only write top level classes to the index
      if (doc.Value.NestedIn is null)
      {
        index.AppendLine($"- [{doc.Value.Name}](./pages/{doc.Value.FullName}.html)");
      }
    }

    index.Append(Utils.DefaultFooter);
    Markdown.ToHtml(index.ToString(), indexFile);
  }
}
