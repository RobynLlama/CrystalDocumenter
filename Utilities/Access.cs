/*
Copyright (C) 2025 Robyn (robyn@mamallama.dev)

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
*/

using System.Reflection;

namespace CrystalDocumenter.Utilities;

public static partial class Utils
{
  public static string GetAccessibility(MethodBase method)
  {
    if (method.IsPublic) return "public";
    if (method.IsPrivate) return "private";
    if (method.IsFamily) return "protected";
    if (method.IsAssembly) return "internal";
    if (method.IsFamilyOrAssembly) return "protected internal";

    return "unknown";
  }

  public static string GetAccessibility(FieldInfo field)
  {
    if (field.IsPublic) return "public";
    if (field.IsPrivate) return "private";
    if (field.IsFamily) return "protected";
    if (field.IsAssembly) return "internal";
    if (field.IsFamilyOrAssembly) return "protected internal";

    return "unknown";
  }
}
