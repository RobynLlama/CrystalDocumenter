/*
Copyright (C) 2025 Robyn (robyn@mamallama.dev)

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
*/

using System;

namespace CrystalDocumenter.Utilities;

public static partial class Utils
{
  public static string HashType(Type thisType)
  {
    return SHA256HashString(GetTypeNameForHashing(thisType));
  }
  public static string GetTypeNameForHashing(Type thisType)
  {
    string input;

    if (thisType.FullName != null)
    {
      input = thisType.FullName;
    }
    else if (thisType.IsGenericParameter)
    {
      input = $"{thisType.DeclaringMethod.GetType().FullName}.{thisType.Name}";
    }
    else
    {
      //Fallback for other cases like anonymous types
      input = thisType.Name;
      if (thisType.DeclaringType is not null)
      {
        input += $"_{thisType.DeclaringType.Name}";
      }
    }

    return input;
  }
}
