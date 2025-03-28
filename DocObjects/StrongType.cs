using System.Text.Json.Serialization;

namespace CrystalDocumenter.DocObjects;

[method: JsonConstructor]
public class StrongType(string Name, string Hash)
{
  [JsonInclude]
  public string Name = Name;

  [JsonInclude]
  public string Hash = Hash;

  public string GetEscapedName() =>
    Name.Replace("<", "\\<").Replace(">", "\\>");
}
