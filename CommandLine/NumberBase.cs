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
    private static readonly Dictionary<string, NumberBase> lookup =
        new(StringComparer.OrdinalIgnoreCase)
        {
            {"hex", NumberBase.Hex},
            {"16", NumberBase.Hex},
            {"decimal", NumberBase.Decimal},
            {"10", NumberBase.Decimal},
            {"binary", NumberBase.Binary},
            {"2", NumberBase.Binary}
        };

    public override object ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is not string stringValue) throw new NotSupportedException("Can't convert value to number base.");
        if (!lookup.TryGetValue(stringValue, out var numberBase))
            throw new InvalidOperationException($"The value '{value}' is not a number base.");
        return numberBase;
    }
}