using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using CrystalDocumenter.Utilities;

namespace CrystalDocumenter.DocObjects;

public class FieldDocumentation
{
  [JsonInclude]
  public string Accessibility;

  [JsonInclude]
  public bool IsStatic;

  [JsonInclude]
  public string FieldName;

  [JsonInclude]
  public string FieldType;

  [JsonInclude]
  public string FieldSummary;

  [JsonConstructor]
  public FieldDocumentation(
  string accessibility,
  bool isStatic,
  string fieldName,
  string fieldType,
  string fieldSummary)
  {
    Accessibility = accessibility;
    IsStatic = isStatic;
    FieldName = fieldName;
    FieldType = fieldType;
    FieldSummary = fieldSummary;
  }

  public FieldDocumentation(FieldInfo field)
  {
    Accessibility = Utils.GetAccessibility(field);
    IsStatic = field.IsStatic;
    FieldName = field.Name ?? "Unknown";
    FieldType = field.FieldType.Name;
    FieldSummary = "No Summary";
  }

  public void Save(StringBuilder data)
  {
    data.AppendLine($"- {Accessibility} **{FieldName}** (_{FieldType}_):\n  - {FieldSummary}");
  }
}
