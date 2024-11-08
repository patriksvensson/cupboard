namespace Cupboard;

[PublicAPI]
public sealed class Chmod
{
    private static readonly Dictionary<Func<Chmod, Permissions>, Dictionary<Permissions, UnixFileMode>> _lookup;

    public SpecialMode Mode { get; }
    public Permissions Owner { get; }
    public Permissions Group { get; }
    public Permissions Other { get; }

    static Chmod()
    {
        _lookup = new()
        {
            [c => c.Owner] = new()
            {
                { Permissions.Read, UnixFileMode.UserRead },
                { Permissions.Write, UnixFileMode.UserWrite },
                { Permissions.Execute, UnixFileMode.UserExecute },
            },
            [c => c.Group] = new()
            {
                { Permissions.Read, UnixFileMode.GroupRead },
                { Permissions.Write, UnixFileMode.GroupWrite },
                { Permissions.Execute, UnixFileMode.GroupExecute },
            },
            [c => c.Other] = new()
            {
                { Permissions.Read, UnixFileMode.OtherRead },
                { Permissions.Write, UnixFileMode.OtherWrite },
                { Permissions.Execute, UnixFileMode.OtherExecute },
            },
        };
    }

    public Chmod(Permissions owner, Permissions group, Permissions other)
        : this(SpecialMode.None, owner, group, other)
    {
    }

    public Chmod(SpecialMode mode, Permissions owner, Permissions group, Permissions other)
    {
        Mode = mode;
        Owner = owner;
        Group = group;
        Other = other;
    }

    public static Chmod Parse(string pattern)
    {
        return ChmodParser.Parse(pattern);
    }

    public Permissions GetPermissions(ChmodClass @class)
    {
        return @class switch
        {
            ChmodClass.Owner => Owner,
            ChmodClass.Group => Group,
            ChmodClass.Other => Other,
            _ => throw new NotSupportedException("Invalid chmod reference."),
        };
    }

    public UnixFileMode ToFileAccessPermissions()
    {
        var permissions = default(UnixFileMode);
        foreach (var map in _lookup)
        {
            var permission = map.Key(this);
            foreach (var p in map.Value)
            {
                if (permission.HasFlag(p.Key))
                {
                    permissions |= p.Value;
                }
            }
        }

        return permissions;
    }

    public string ToString(ChmodFormatting formatting)
    {
        return ChmodFormatter.Format(this, formatting);
    }

    public override string ToString()
    {
        return ChmodFormatter.Format(this, ChmodFormatting.Numeric);
    }
}
