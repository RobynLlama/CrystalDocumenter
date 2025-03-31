/*
Copyright (C) 2025 Robyn (robyn@mamallama.dev)

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
*/

using System.Text.Json.Serialization;

namespace CrystalDocumenter.DocObjects;

[method: JsonConstructor]
public class StrongType(string Name, string FullName)
{
  [JsonInclude]
  public string Name = Name;

  [JsonInclude]
  public string FullName = FullName;
}
