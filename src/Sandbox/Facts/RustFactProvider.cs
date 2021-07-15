using System;
using System.Collections.Generic;
using Cupboard;
using Spectre.Console.Cli;
using Spectre.IO;

namespace Sandbox
{
    public sealed class RustFactProvider : IFactProvider
    {
        private readonly IFileSystem _fileSystem;
        private readonly IEnvironment _environment;

        public RustFactProvider(IFileSystem fileSystem, IEnvironment environment)
        {
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }

        IEnumerable<(string Name, object Value)> IFactProvider.GetFacts(IRemainingArguments args)
        {
            var path = new DirectoryPath("~/.cargo").MakeAbsolute(_environment);
            yield return ("rust.installed", _fileSystem.Exist(path));
        }
    }
}
