using System.ComponentModel;

namespace TextProcessorApp.Core.Extensions;
public static class EnumExtensions
{
    public static string GetEnumDescription(this Enum value)
        => (value.GetType()
            .GetField(value.ToString())
            ?.GetCustomAttributes(typeof(DescriptionAttribute), false)
            .FirstOrDefault() as DescriptionAttribute)?.Description ?? value.ToString();
}
