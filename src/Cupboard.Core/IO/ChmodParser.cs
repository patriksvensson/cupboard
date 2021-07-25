using System;
using System.Collections.Generic;

namespace Cupboard
{
    public static class ChmodParser
    {
        public static Chmod Parse(string pattern)
        {
            if (pattern == null)
            {
                throw new ArgumentNullException(nameof(pattern));
            }

            if (pattern.Length != 3 && pattern.Length != 4)
            {
                throw new ArgumentException("Invalid pattern.", nameof(pattern));
            }

            var mode = SpecialMode.None;
            if (pattern.Length == 4)
            {
                mode = ParseSpecialMode(pattern[0]);
                pattern = pattern.Substring(1, 3);
            }

            var queue = new Queue<ChmodClass>(new[] { ChmodClass.Owner, ChmodClass.Group, ChmodClass.Other });
            var map = new Dictionary<ChmodClass, Permissions>
            {
                { ChmodClass.Owner, Permissions.None },
                { ChmodClass.Group, Permissions.None },
                { ChmodClass.Other, Permissions.None },
            };

            foreach (var token in pattern)
            {
                var current = queue.Dequeue();
                if (char.IsNumber(token))
                {
                    var n = int.Parse(token.ToString());
                    map[current] = (Permissions)n;
                }
            }

            return new Chmod(
                mode,
                map[ChmodClass.Owner],
                map[ChmodClass.Group],
                map[ChmodClass.Other]);
        }

        private static SpecialMode ParseSpecialMode(char token)
        {
            if (char.IsNumber(token))
            {
                return (SpecialMode)int.Parse(token.ToString());
            }

            return SpecialMode.None;
        }
    }
}
