/*
Copyright (C) 2025 Robyn (robyn@mamallama.dev)

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
*/

using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using CrystalDocumenter.Utilities;

namespace CrystalDocumenter.DocObjects;

public class FieldDocumentation
{
  [JsonInclude]
  public string Accessibility;

  [JsonInclude]
  public bool IsStatic;

  [JsonInclude]
  public string FieldName;

  [JsonInclude]
  public string FieldType;

  [JsonInclude]
  public string FieldSummary;

  [JsonConstructor]
  public FieldDocumentation(
  string accessibility,
  bool isStatic,
  string fieldName,
  string fieldType,
  string fieldSummary)
  {
    Accessibility = accessibility;
    IsStatic = isStatic;
    FieldName = fieldName;
    FieldType = fieldType;
    FieldSummary = fieldSummary;
  }

  public FieldDocumentation(FieldInfo field)
  {
    Accessibility = Utils.GetAccessibility(field);
    IsStatic = field.IsStatic;
    FieldName = Strategy.DefaultNamingStrategy(field);
    FieldType = field.FieldType.Name;
    FieldSummary = string.Empty;
  }

  public void Save(StringBuilder data)
  {
    data.AppendLine($"- {Accessibility} **{FieldName}** (_{FieldType}_):\n  - {FieldSummary}");
  }
}
