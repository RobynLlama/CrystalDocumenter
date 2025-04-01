/*
Copyright (C) 2025 Robyn (robyn@mamallama.dev)

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
*/

using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using CrystalDocumenter.Utilities;

namespace CrystalDocumenter.DocObjects;

public class MethodDocumentation
{
  [JsonInclude]
  public string MethodName;

  [JsonInclude]
  public string Accessibility;

  [JsonInclude]
  public string MethodSummary;

  [JsonInclude]
  public bool IsStatic;

  [JsonInclude]
  public string ReturnType;

  [JsonInclude]
  public List<ParameterDocumentation> Parameters = [];

  [JsonConstructor]
  public MethodDocumentation(
    string accessibility,
    string methodName,
    string methodSummary,
    bool isStatic,
    string returnType,
    List<ParameterDocumentation> parameters)
  {
    Accessibility = accessibility;
    MethodName = methodName;
    MethodSummary = methodSummary;
    IsStatic = isStatic;
    ReturnType = returnType;
    Parameters = parameters;
  }

  public MethodDocumentation(MethodInfo method)
  {
    MethodName = Strategy.DefaultNamingStrategy(method);
    Accessibility = Utils.GetAccessibility(method);
    MethodSummary = string.Empty;
    IsStatic = method.IsStatic;
    ReturnType = method.ReturnType.Name;

    foreach (var param in method.GetParameters())
    {
      Parameters.Add(new(param));
    }
  }

  public void Save(StringBuilder data)
  {

    string signature = $"{Accessibility} {(IsStatic ? "static " : "")}{ReturnType} {MethodName}(";

    for (int i = 0; i < Parameters.Count; i++)
    {
      if (i > 0) signature += ", ";
      signature += $"{Parameters[i].ParameterType} {Parameters[i].ParameterName}";
    }
    signature += ")";

    data.AppendLine($"\n### {MethodName}\n");

    data.AppendLine(MethodSummary + "\n");

    data.AppendLine("```cs");
    data.AppendLine(signature);
    data.AppendLine("```\n");

    if (Parameters.Count > 0)
    {
      data.AppendLine("Method Parameters\n");
      data.AppendLine("<dl>");

      foreach (var item in Parameters)
      {
        data.AppendLine($"<dt>{item.ParameterName}</dt><dd>{item.ParameterSummary}</dd>");
      }

      data.AppendLine("</dl>");
    }
  }
}
