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
      // Fallback for other cases like anonymous types
      input = thisType.Name;
      if (thisType.DeclaringType is not null)
      {
        input += $"_{thisType.DeclaringType.Name}";
      }
    }

    return input;
  }
}
