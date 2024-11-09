using System.ComponentModel;

namespace TextProcessorApp.Contracts.Enums;
public enum TransformType
{
    [Description("uppercase")]
    UpperCase,

    [Description("lowercase")]
    LowerCase,
    
    [Description("dash-replacement")]
    DashReplacement
}
