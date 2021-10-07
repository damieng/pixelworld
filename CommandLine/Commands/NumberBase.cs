using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace CommandLine.Commands
{
    public enum NumberBase
    {
        Binary = 2,
        Decimal = 10,
        Hex = 16,
    }

    public sealed class NumberBaseConverter : TypeConverter
    {
        private readonly Dictionary<string, NumberBase> lookup;

        public NumberBaseConverter()
        {
            lookup = new Dictionary<string, NumberBase>(StringComparer.OrdinalIgnoreCase)
            {
                { "hex", NumberBase.Hex },
                { "16", NumberBase.Hex },
                { "decimal", NumberBase.Decimal },
                { "10", NumberBase.Decimal },
                { "binary", NumberBase.Binary },
                { "2", NumberBase.Binary },
            };
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string stringValue)
            {
                var result = lookup.TryGetValue(stringValue, out var numberBase);
                if (!result)
                {
                    const string format = "The value '{0}' is not a number base.";
                    var message = string.Format(CultureInfo.InvariantCulture, format, value);
                    throw new InvalidOperationException(message);
                }
                return numberBase;
            }
            throw new NotSupportedException("Can't convert value to number base.");
        }
    }
}
