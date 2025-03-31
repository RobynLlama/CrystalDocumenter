/*
Copyright (C) 2025 Robyn (robyn@mamallama.dev)

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
*/

using System;
using System.Collections.Generic;

namespace CrystalDocumenter;

partial class DocumenterProgram
{
  internal static string InputFile = string.Empty;
  internal static string OutputDir = "Documentation";
  public static void Usage()
  {
    int paddingRight = 20;
    string paddingLeft = new(' ', 2);

    void WriteSwitchInfo(string switches, string description)
    {
      Console.Write(paddingLeft + switches.PadRight(paddingRight));
      Console.WriteLine("| " + description);
    }

    Console.WriteLine();
    Console.WriteLine("Options: ");

    WriteSwitchInfo("-h, --help", "Show this screen");
    WriteSwitchInfo("-i, --input", "set the input file path");
    WriteSwitchInfo("-o, --output", "set the output directory, default: Documentation");
  }
  internal static void HandleArguments(in string[] args)
  {
    Queue<string> argStream = new(args);

    while (argStream.Count > 0)
    {
      var current = argStream.Dequeue();

      switch (current)
      {
        case "--help":
        case "-h":
          Usage();
          Environment.Exit(0);
          break;
        case "--input":
        case "-i":
          if (argStream.Count == 0)
            throw new ArgumentException("Specify a value after --input/-i");

          InputFile = argStream.Dequeue();
          break;
        case "--output":
        case "-o":
          if (argStream.Count == 0)
            throw new ArgumentException("Specify a value after --output/-o");

          OutputDir = argStream.Dequeue();
          break;
        default:
          throw new InvalidOperationException($"Unknown switch {current}");
      }
    }
  }
}
