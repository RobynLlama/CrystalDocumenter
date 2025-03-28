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
