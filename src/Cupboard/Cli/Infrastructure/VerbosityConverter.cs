using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace Cupboard.Internal;

internal sealed class VerbosityConverter : TypeConverter
{
    private readonly Dictionary<string, Verbosity> _lookup;

    public VerbosityConverter()
    {
        _lookup = new Dictionary<string, Verbosity>(StringComparer.OrdinalIgnoreCase)
        {
            { "q", Verbosity.Quiet },
            { "quiet", Verbosity.Quiet },
            { "m", Verbosity.Minimal },
            { "minimal", Verbosity.Minimal },
            { "n", Verbosity.Normal },
            { "normal", Verbosity.Normal },
            { "v", Verbosity.Verbose },
            { "verbose", Verbosity.Verbose },
            { "d", Verbosity.Diagnostic },
            { "diagnostic", Verbosity.Diagnostic },
        };
    }

    /// <inheritdoc/>
    public override object ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is string stringValue)
        {
            var result = _lookup.TryGetValue(stringValue, out var verbosity);
            if (!result)
            {
                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "The value '{0}' is not a valid verbosity.",
                        value));
            }

            return verbosity;
        }

        throw new NotSupportedException("Can't convert value to verbosity.");
    }
}
