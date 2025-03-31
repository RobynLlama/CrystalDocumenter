using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using CrystalDocumenter.Utilities;

namespace CrystalDocumenter.DocObjects;

public class PropertyDocumentation
{
  [JsonInclude]
  public string GetterAccessibility;

  [JsonInclude]
  public string SetterAccessibility;

  [JsonInclude]
  public bool IsStatic;

  [JsonInclude]
  public string PropertyName;

  [JsonInclude]
  public string PropertyType;

  [JsonInclude]
  public string PropertySummary;

  [JsonConstructor]
  public PropertyDocumentation(
    string getterAccessibility,
    string setterAccessibility,
    bool isStatic,
    string propertyName,
    string propertyType,
    string propertySummary)
  {
    GetterAccessibility = getterAccessibility;
    SetterAccessibility = setterAccessibility;
    IsStatic = isStatic;
    PropertyName = propertyName;
    PropertyType = propertyType;
    PropertySummary = propertySummary;
  }

  public PropertyDocumentation(PropertyInfo prop)
  {

    MethodInfo? getMethod = prop.GetGetMethod(true);
    MethodInfo? setMethod = prop.GetSetMethod(true);

    if (getMethod is not null)
    {
      GetterAccessibility = Utils.GetAccessibility(getMethod);
      IsStatic = getMethod.IsStatic;
    }
    else
    {
      GetterAccessibility = "???";
      IsStatic = false;
    }

    SetterAccessibility = setMethod is not null ? Utils.GetAccessibility(setMethod) : "READONLY";

    PropertyName = Strategy.DefaultNamingStrategy(prop);
    PropertyType = prop.PropertyType.Name;
    PropertySummary = string.Empty;
  }

  public void Save(StringBuilder data)
  {

    string sig = $"{GetterAccessibility} {PropertyType} {PropertyName} {{ {GetterAccessibility} get; ";

    if (SetterAccessibility != "READONLY")
      sig += $"{SetterAccessibility} set; ";

    sig += "}";

    data.AppendLine($"- {sig}\n  - {PropertySummary}");
  }
}
