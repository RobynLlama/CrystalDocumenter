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
