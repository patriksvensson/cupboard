using System;
using Spectre.IO;

namespace Cupboard
{
    public sealed class Download : Resource
    {
        public Uri? Url { get; set; }
        public Chmod? Permissions { get; set; }
        public Path? Destination { get; set; }

        public Download(string name)
            : base(name)
        {
            if (Uri.TryCreate(name, UriKind.Absolute, out var result))
            {
                Url = result;
            }
        }
    }
}
