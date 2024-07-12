namespace Cupboard;

/// <summary>
/// A type converter for <see cref="DirectoryPath"/>.
/// </summary>
internal sealed class DirectoryPathConverter : TypeConverter
{
    /// <inheritdoc/>
    public override object ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is string stringValue)
        {
            return new DirectoryPath(stringValue);
        }

        throw new NotSupportedException("Can't convert value to file path.");
    }
}
