namespace Cupboard;

/// <summary>
/// A type converter for <see cref="FilePath"/>.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.Members)]
internal sealed class FilePathConverter : TypeConverter
{
    /// <inheritdoc/>
    public override object ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is string stringValue)
        {
            return new FilePath(stringValue);
        }

        throw new NotSupportedException("Can't convert value to file path.");
    }
}
