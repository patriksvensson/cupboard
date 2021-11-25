using System;
using System.Collections.Generic;
using Cupboard.Internal;
using Mono.Unix;

namespace Cupboard;

public sealed class Chmod
{
    private static readonly Dictionary<Func<Chmod, Permissions>, Dictionary<Permissions, FileAccessPermissions>> _lookup;

    public SpecialMode Mode { get; }
    public Permissions Owner { get; }
    public Permissions Group { get; }
    public Permissions Other { get; }

    static Chmod()
    {
        _lookup = new Dictionary<Func<Chmod, Permissions>, Dictionary<Permissions, FileAccessPermissions>>
        {
            [c => c.Owner] = new Dictionary<Permissions, FileAccessPermissions>
            {
                { Permissions.Read, FileAccessPermissions.UserRead },
                { Permissions.Write, FileAccessPermissions.UserWrite },
                { Permissions.Execute, FileAccessPermissions.UserExecute },
            },
            [c => c.Group] = new Dictionary<Permissions, FileAccessPermissions>
            {
                { Permissions.Read, FileAccessPermissions.GroupRead },
                { Permissions.Write, FileAccessPermissions.GroupWrite },
                { Permissions.Execute, FileAccessPermissions.GroupExecute },
            },
            [c => c.Other] = new Dictionary<Permissions, FileAccessPermissions>
            {
                { Permissions.Read, FileAccessPermissions.OtherRead },
                { Permissions.Write, FileAccessPermissions.OtherWrite },
                { Permissions.Execute, FileAccessPermissions.OtherExecute },
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

    public FileAccessPermissions ToFileAccessPermissions()
    {
        var permissions = default(FileAccessPermissions);
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