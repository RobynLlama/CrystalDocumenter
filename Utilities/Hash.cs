/*
Copyright (C) 2025 Robyn (robyn@mamallama.dev)

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
*/

using System.Security.Cryptography;
using System.Text;

namespace CrystalDocumenter.Utilities;

public static partial class Utils
{
  public static string SHA256HashString(string input)
  {
    byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
    StringBuilder builder = new();
    foreach (byte b in bytes)
    {
      builder.Append(b.ToString("x2"));
    }
    return builder.ToString();
  }
}
