using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace CommandLine;

public enum NumberBase
{
    Binary = 2,
    Decimal = 10,
    Hex = 16
}

public sealed class NumberBaseConverter : TypeConverter
{
    private static readonly Dictionary<String, NumberBase> lookup =
        new(StringComparer.OrdinalIgnoreCase)
        {
            {"hex", NumberBase.Hex},
            {"16", NumberBase.Hex},
            {"decimal", NumberBase.Decimal},
            {"10", NumberBase.Decimal},
            {"binary", NumberBase.Binary},
            {"2", NumberBase.Binary}
        };

    public override Object ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, Object value)
    {
        if (value is not String stringValue) throw new NotSupportedException("Can't convert value to number base.");
        return lookup.TryGetValue(stringValue, out var numberBase)
            ? numberBase
            : throw new InvalidOperationException($"The value '{value}' is not a number base.");
    }
}