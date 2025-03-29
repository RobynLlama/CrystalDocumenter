using System;
using System.Reflection;
using CrystalDocumenter.Utilities;

namespace CrystalDocumenter.DocObjects;

public static partial class Strategy
{

  /// <summary>
  /// Creates a name that won't be mangled by Markdig, should only
  /// be used for display, this is <b>NOT</b> file safe
  /// </summary>
  /// <param name="name"></param>
  /// <returns></returns>
  public static string GetEscapedDisplayOnlyName(string name)
  {
    return name.Replace("<", "\\<").Replace(">", "\\>").Replace("`", "\\`");
  }

  /// <summary>
  /// Creates a file and XML safe name from a member name
  /// </summary>
  /// <param name="name"></param>
  /// <returns></returns>
  private static string DefaultMemberNamingStrategy(string name)
  {
    return name.Replace("<", "_u002B_").Replace(">", "_u003C_");
  }

  public static string DefaultNamingStrategy(Type thisType) => DefaultMemberNamingStrategy(Utils.GetTypeNameForHashing(thisType));
  public static string DefaultNamingStrategy(PropertyInfo info) => DefaultMemberNamingStrategy(info.Name);
  public static string DefaultNamingStrategy(FieldInfo info) => DefaultMemberNamingStrategy(info.Name);
  public static string DefaultNamingStrategy(MethodInfo info) => DefaultMemberNamingStrategy(info.Name);
  public static string DefaultNamingStrategy(ParameterInfo info)
  {
    if (info.Name is string name)
      return DefaultMemberNamingStrategy(name);

    return DefaultMemberNamingStrategy($"{info.Member.Name}.__{info.GetHashCode()}");
  }
}
