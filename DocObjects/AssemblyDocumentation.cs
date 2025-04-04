/*
Copyright (C) 2025 Robyn (robyn@mamallama.dev)

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Xml;
using CrystalDocumenter.Utilities;
using Markdig;
using Markdig.Extensions.AutoIdentifiers;

namespace CrystalDocumenter.DocObjects;

public class AssemblyDocumentation
{
  public readonly Assembly Origin;
  public static readonly MarkdownPipeline Pipeline;

  static AssemblyDocumentation()
  {
    Pipeline = new MarkdownPipelineBuilder()
    .UseAdvancedExtensions()
    .UseAutoIdentifiers(AutoIdentifierOptions.GitHub)
    .Build();
  }

  protected readonly Dictionary<string, TypeDocumentation> TypeInfos = [];
  public TypeDocumentation? GetOrAddType(Type thisType)
  {
    if (thisType.Assembly != Origin)
      return null;

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

    Console.WriteLine("-- Documenting Types --");

    foreach (Type type in Origin.GetTypes())
    {
      if (GetOrAddType(type) is not TypeDocumentation thing)
        continue;

      Utils.BasicLogger.LogInfo("Found", thing.Name, 1);

      //Don't write nested classes in the index right now
      if (thing.NestedIn is null)
        index.WriteLine($"- [{thing.Name}](./intermediary/{thing.FullName}.json)");
    }

    Console.WriteLine("-- Generating Intermediary Media --");

    foreach (var item in TypeInfos)
    {
      FileInfo output = new(Path.Combine(intermediary.FullName, $"{item.Value.FullName}.json"));

      if (output.Exists)
      {
        Utils.BasicLogger.LogInfo("Skipping", item.Value.Name, 1);
        continue;
      }

      Utils.BasicLogger.LogSuccess("Writing", output.Name, 1);

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

    Console.WriteLine("-- Building Static Pages --");

    foreach (var file in files)
    {
      using StreamReader reader = new(file.FullName);
      var jsonText = reader.ReadToEnd();

      if (JsonSerializer.Deserialize<TypeDocumentation>(jsonText) is not TypeDocumentation item)
        throw new Exception($"Unable to deserialize a TypeDocumentation from {file.FullName}");

      Utils.BasicLogger.LogInfo("Reading", item.Name, 1);
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
      Markdown.ToHtml(data.ToString(), writer, pipeline: Pipeline);

      Utils.BasicLogger.LogSuccess("Wrote Page", $"{output.Name}{Path.DirectorySeparatorChar}{doc.Value.FullName}.html", 1);

      //Only write top level classes to the index
      if (doc.Value.NestedIn is null)
      {
        index.AppendLine($"- [{doc.Value.Name}](./pages/{doc.Value.FullName}.html)");
      }
    }

    index.Append(Utils.DefaultFooter);
    Markdown.ToHtml(index.ToString(), indexFile, pipeline: Pipeline);

    Utils.BasicLogger.LogSuccess("Wrote Page", "Index.html", 1);
  }

  public void BuildXMLDoc(string output = "Documentation")
  {
    FileInfo file = new(Path.Combine(output, $"{Origin.GetName().Name}.xml"));
    using XmlWriter xml = XmlWriter.Create(file.FullName, new() { Indent = true });

    //setup the header and junk
    xml.WriteStartDocument();
    xml.WriteStartElement("doc");

    xml.WriteStartElement("assembly");
    xml.WriteElementString("name", Origin.GetName().Name);
    xml.WriteEndElement();

    xml.WriteStartElement("members");

    Console.WriteLine("-- Writing XMLDoc --");

    foreach (var type in TypeInfos.Values)
    {
      type.ToXMLElement(xml);
      Utils.BasicLogger.LogSuccess("Wrote Member", type.Name, 1);
    }

    xml.WriteEndElement();
    xml.WriteEndElement();
    xml.WriteEndDocument();
  }
}
