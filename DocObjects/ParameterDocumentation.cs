/*
Copyright (C) 2025 Robyn (robyn@mamallama.dev)

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
*/

using System.Reflection;
using System.Text.Json.Serialization;

namespace CrystalDocumenter.DocObjects;

public class ParameterDocumentation
{
  [JsonInclude]
  public string ParameterName;

  [JsonInclude]
  public string ParameterType;

  [JsonInclude]
  public string ParameterSummary;

  [JsonConstructor]
  public ParameterDocumentation(string ParameterName, string ParameterType, string ParameterSummary)
  {
    this.ParameterName = ParameterName;
    this.ParameterType = ParameterType;
    this.ParameterSummary = ParameterSummary;
  }

  public ParameterDocumentation(ParameterInfo param)
  {
    ParameterName = Strategy.DefaultNamingStrategy(param);
    ParameterType = param.ParameterType.FullName ?? param.ParameterType.Name;
    ParameterSummary = string.Empty;
  }
}
