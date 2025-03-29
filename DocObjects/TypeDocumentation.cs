using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
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
  public string Namespace = "None";

  [JsonInclude]
  public string TypeSummary = "No Summary";

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
    string Namespace,
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
    this.Namespace = Namespace;
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
    Namespace = thisType.Namespace ?? "None";
    FullName = Strategy.DefaultNamingStrategy(thisType);
    InfoHash = Utils.HashType(thisType);
    DescribedType = thisType;

    if (thisType.BaseType is Type parent)
    {
      var p = Origin.GetOrAddType(parent);
      Parent = new(p.Name, p.FullName);
    }

    if (thisType.IsNested && thisType.DeclaringType is not null)
    {
      var nested = Origin.GetOrAddType(thisType.DeclaringType);
      NestedIn = new(nested.Name, nested.FullName);
      nested.NestedTypes.Add(new(Name, FullName));
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
    data.AppendLine($"- **Namespace**: {Namespace}");
    data.AppendLine($"- **FullName**: {FullName}");
    data.AppendLine($"- **Summary**: {TypeSummary}");

    if (NestedIn is not null)
      data.AppendLine($"- **Nested In**: [{Strategy.GetEscapedDisplayOnlyName(NestedIn.Name)}]({NestedIn.FullName}.html)");

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

  private bool ShouldRecordMethod(MethodInfo method)
  {
    if (method.DeclaringType is null)
      return true;

    return method.DeclaringType == DescribedType;
  }
}
