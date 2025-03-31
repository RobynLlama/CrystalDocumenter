using System.Reflection;
using System.Text.Json.Serialization;

namespace CrystalDocumenter.DocObjects;

public class ParameterDocumentation
{
  [JsonInclude]
  public string ParameterName;

  [JsonInclude]
  public string ParameterType;

  [JsonInclude]
  public string ParameterSummary;

  [JsonConstructor]
  public ParameterDocumentation(string ParameterName, string ParameterType, string ParameterSummary)
  {
    this.ParameterName = ParameterName;
    this.ParameterType = ParameterType;
    this.ParameterSummary = ParameterSummary;
  }

  public ParameterDocumentation(ParameterInfo param)
  {
    ParameterName = Strategy.DefaultNamingStrategy(param);
    ParameterType = param.ParameterType.FullName ?? param.ParameterType.Name;
    ParameterSummary = string.Empty;
  }
}
