using System;

namespace CrystalDocumenter.Utilities;

public static partial class Utils
{
  public static class BasicLogger
  {
    public static ConsoleColor ForegroundBad = ConsoleColor.Red;
    public static ConsoleColor ForegroundWarn = ConsoleColor.Yellow;
    public static ConsoleColor ForegroundInfo = ConsoleColor.Blue;
    public static ConsoleColor ForegroundGood = ConsoleColor.Green;
    private static bool s_beQuiet = false;

    private static void LogItemColor(string callout, string message, ConsoleColor calloutColor, int indentLevel)
    {
      if (s_beQuiet)
        return;

      var space = indentLevel * 2;

      Console.Write(new string(' ', space));
      Console.Write('[');
      Console.ForegroundColor = calloutColor;
      Console.Write(callout);
      Console.ResetColor();
      Console.Write("] ");
      Console.WriteLine(message);
    }

    public static void LogError(string callout, string message, int indentLevel = 0) => LogItemColor(callout, message, ForegroundBad, indentLevel);
    public static void LogWarning(string callout, string message, int indentLevel = 0) => LogItemColor(callout, message, ForegroundWarn, indentLevel);
    public static void LogInfo(string callout, string message, int indentLevel = 0) => LogItemColor(callout, message, ForegroundInfo, indentLevel);
    public static void LogSuccess(string callout, string message, int indentLevel = 0) => LogItemColor(callout, message, ForegroundGood, indentLevel);
    public static void BeQuiet() => s_beQuiet = true;
  }
}
