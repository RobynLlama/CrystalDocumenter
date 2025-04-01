/*
Copyright (C) 2025 Robyn (robyn@mamallama.dev)

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
*/

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml;
using CrystalDocumenter.DocObjects;
using CrystalDocumenter.Utilities;

namespace CrystalDocumenter;

public class TypeDocumentation
{
  public readonly AssemblyDocumentation? Origin;

  [JsonInclude]
  public StrongType? Parent;

  [JsonInclude]
  public StrongType? NestedIn;

  [JsonInclude]
  public string Name;

  [JsonInclude]
  public string FullName;

  [JsonInclude]
  public string TypeNamespace = string.Empty;

  [JsonInclude]
  public string TypeSummary = string.Empty;

  [JsonInclude]
  public string InfoHash;

  [JsonInclude]
  public List<MethodDocumentation> MethodSignatures = [];

  [JsonInclude]
  public List<PropertyDocumentation> Properties = [];

  [JsonInclude]
  public List<FieldDocumentation> Fields = [];

  [JsonInclude]
  public List<StrongType> Implements = [];

  [JsonInclude]
  public List<StrongType> NestedTypes = [];

  public readonly Type? DescribedType;

  [JsonConstructor]
  public TypeDocumentation(
    StrongType? parent,
    StrongType? nestedIn,
    string name,
    string fullName,
    string infoHash,
    string typeNamespace,
    string typeSummary,
    List<MethodDocumentation> methodSignatures,
    List<PropertyDocumentation> properties,
    List<FieldDocumentation> fields,
    List<StrongType> implements,
    List<StrongType> nestedTypes)
  {
    Parent = parent;
    NestedIn = nestedIn;
    Name = name;
    TypeNamespace = typeNamespace;
    FullName = fullName;
    InfoHash = infoHash;
    TypeSummary = typeSummary;
    MethodSignatures = methodSignatures ?? new List<MethodDocumentation>();
    Properties = properties ?? new List<PropertyDocumentation>();
    Fields = fields ?? new List<FieldDocumentation>();
    Implements = implements ?? new List<StrongType>();
    NestedTypes = nestedTypes ?? new List<StrongType>();
  }

  public TypeDocumentation(AssemblyDocumentation origin, Type thisType)
  {
    Origin = origin;
    Name = thisType.Name;
    TypeNamespace = thisType.Namespace ?? string.Empty;
    FullName = GetFullyQualifiedName(thisType);
    InfoHash = Utils.HashType(thisType);
    DescribedType = thisType;

    if (thisType.BaseType is Type parent)
    {
      if (Origin.GetOrAddType(parent) is TypeDocumentation p)
        Parent = new(p.Name, p.FullName);
    }

    if (thisType.IsNested && thisType.DeclaringType is not null)
    {
      if (Origin.GetOrAddType(thisType.DeclaringType) is TypeDocumentation nested)
      {
        NestedIn = new(nested.Name, nested.FullName);
        nested.NestedTypes.Add(new(Name, FullName));
      }
    }

    string GetFullyQualifiedName(Type type)
    {
      Type? DeclaringType = type.DeclaringType;
      string? typeName = string.Empty;
      char divider = ':';

      while (DeclaringType is not null)
      {
        if (typeName == string.Empty)
        {
          //we're the first level of type so only add our name
          //to the list
          typeName = DeclaringType.FullName;
        }
        else
          typeName = $"{DeclaringType.FullName}{divider}{typeName}";

        //set the divider for the next item to consume
        divider = DeclaringType.IsNested ? '+' : '.';
        //set the next item
        DeclaringType = DeclaringType.DeclaringType;
      }

      var fn = Strategy.DefaultNamingStrategy(type);

      if (typeName == string.Empty)
        return fn;

      return type.IsNested ? $"{typeName}+{fn}" : $"{typeName}.{fn}";
    }

    //Record methods
    foreach (MethodInfo method in DescribedType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
    {
      if (!ShouldRecordMethod(method))
        continue;

      MethodSignatures.Add(new(method));
    }

    //Record fields
    foreach (FieldInfo field in DescribedType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
      Fields.Add(new(field));

    //Record props
    foreach (PropertyInfo property in DescribedType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
      Properties.Add(new(property));

    //Record interfaces
    foreach (var inters in DescribedType.GetInterfaces())
      Implements.Add(new(inters.Name, Strategy.DefaultNamingStrategy(inters)));
  }

  public void Save(StringBuilder data)
  {
    data.AppendLine($"# {Name}\n");
    data.AppendLine("## Navigation");
    data.AppendLine($"- [Index ↰](../Index.html)\n");
    data.AppendLine($"- [Member Info](#member-info)");
    data.AppendLine($"- [Fields](#fields)");
    data.AppendLine($"- [Properties](#properties)");
    data.AppendLine($"- [Methods](#methods)");

    data.AppendLine("## Member Info");
    data.AppendLine($"- **Namespace**: {TypeNamespace}");
    data.AppendLine($"- **FullName**: {FullName}");
    data.AppendLine($"- **Summary**: {TypeSummary}");

    if (NestedIn is not null)
      data.AppendLine($"- **Nested In**: [{Strategy.GetEscapedDisplayOnlyName(NestedIn.Name)} ↩]({NestedIn.FullName}.html)");

    if (Parent is not null)
      data.AppendLine($"- **Parent**: {Strategy.GetEscapedDisplayOnlyName(Parent.Name)}");

    if (NestedTypes.Count > 0)
    {
      data.AppendLine("- **Nested Types**:");
      foreach (var nest in NestedTypes)
      {
        data.AppendLine($"  - [{Strategy.GetEscapedDisplayOnlyName(nest.Name)}]({nest.FullName}.html)");
      }
    }

    if (Implements.Count > 0)
    {
      data.AppendLine("- **Implements**:");
      foreach (var item in Implements)
        data.AppendLine($"  - {Strategy.GetEscapedDisplayOnlyName(item.Name)}");
    }

    if (Fields.Count > 0)
    {
      data.AppendLine("\n## Fields\n");
      foreach (var item in Fields)
        item.Save(data);
    }

    if (Properties.Count > 0)
    {
      data.AppendLine("\n## Properties\n");
      foreach (var item in Properties)
        item.Save(data);
    }

    if (MethodSignatures.Count > 0)
    {
      data.AppendLine("\n## Methods\n");
      foreach (var item in MethodSignatures)
        item.Save(data);
    }

  }

  public void ToXMLElement(XmlWriter xml)
  {
    //Check if this type is documented at all
    if (string.IsNullOrEmpty(TypeSummary))
      return;

    //write the type element
    xml.WriteStartElement("member");
    xml.WriteAttributeString("name", $"T:{FullName}");
    xml.WriteElementString("summary", TypeSummary);
    xml.WriteEndElement();

    //write fields
    foreach (var field in Fields)
    {

      if (string.IsNullOrEmpty(field.FieldSummary))
        continue;

      xml.WriteStartElement("member");
      xml.WriteAttributeString("name", $"F:{FullName}.{field.FieldName}");
      xml.WriteElementString("summary", field.FieldSummary);
      xml.WriteEndElement();
    }

    //write props
    foreach (var prop in Properties)
    {

      if (string.IsNullOrEmpty(prop.PropertySummary))
        continue;

      xml.WriteStartElement("member");
      xml.WriteAttributeString("name", $"P:{FullName}.{prop.PropertyName}");
      xml.WriteElementString("summary", prop.PropertySummary);
      xml.WriteEndElement();
    }

    //write methods
    foreach (var method in MethodSignatures)
    {

      if (string.IsNullOrEmpty(method.MethodSummary))
        continue;

      string? paramString = null;

      foreach (var param in method.Parameters)
        if (paramString is null)
          paramString = param.ParameterType;
        else
          paramString = $"{paramString},{param.ParameterType}";

      if (paramString is not null)
        paramString = $"({paramString})";

      xml.WriteStartElement("member");
      xml.WriteAttributeString("name", $"M:{FullName}.{method.MethodName}{paramString}");
      xml.WriteElementString("summary", method.MethodSummary);

      foreach (var param in method.Parameters)
      {
        xml.WriteStartElement("param");
        xml.WriteAttributeString("name", param.ParameterName);
        xml.WriteString(param.ParameterSummary);
        xml.WriteEndElement();
      }
      xml.WriteEndElement();
    }
  }

  private bool ShouldRecordMethod(MethodInfo method)
  {
    if (method.DeclaringType is null)
      return true;

    return method.DeclaringType == DescribedType;
  }
}
