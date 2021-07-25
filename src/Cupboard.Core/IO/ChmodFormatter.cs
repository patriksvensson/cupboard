using System;

namespace Cupboard.Internal
{
    internal static class ChmodFormatter
    {
        public static string Format(Chmod chmod, ChmodFormatting formatting)
        {
            return formatting switch
            {
                ChmodFormatting.Numeric => FormatNumeric(chmod),
                ChmodFormatting.Symbolic => FormatVerbose(chmod),
                _ => throw new NotSupportedException("Formatting option is not supported."),
            };
        }

        private static string FormatNumeric(Chmod chmod)
        {
            return string.Concat(
                ((int)chmod.Mode).ToString(),
                Format(chmod.Owner, ChmodFormatting.Numeric),
                Format(chmod.Group, ChmodFormatting.Numeric),
                Format(chmod.Other, ChmodFormatting.Numeric));
        }

        private static string FormatVerbose(Chmod chmod)
        {
            return string.Concat(
                Format(chmod.Owner, ChmodFormatting.Symbolic),
                Format(chmod.Group, ChmodFormatting.Symbolic),
                Format(chmod.Other, ChmodFormatting.Symbolic));
        }

        private static string Format(Permissions permissions, ChmodFormatting formatting)
        {
            if (formatting == ChmodFormatting.Numeric)
            {
                return ((int)permissions).ToString();
            }

            if (formatting == ChmodFormatting.Symbolic)
            {
                return string.Concat(
                    (permissions & Permissions.Read) == Permissions.Read ? "r" : "-",
                    (permissions & Permissions.Write) == Permissions.Write ? "w" : "-",
                    (permissions & Permissions.Execute) == Permissions.Execute ? "x" : "-");
            }

            throw new NotSupportedException("Formatting option is not supported.");
        }
    }
}
