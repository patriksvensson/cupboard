using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Cupboard
{
    public sealed class CupboardHost
    {
        private readonly IAnsiConsole _console;
        private readonly IHost _host;

        internal CupboardHost(IAnsiConsole console, IHost host)
        {
            _console = console ?? throw new ArgumentNullException(nameof(console));
            _host = host ?? throw new ArgumentNullException(nameof(host));
        }

        public static CupboardHostBuilder CreateBuilder()
        {
            return new CupboardHostBuilder();
        }

        public int Run(string[] args)
        {
            try
            {
                var app = _host.Services.GetRequiredService<ICommandApp>();
                return app.Run(args);
            }
            catch (Exception ex)
            {
                _console.WriteLine();
                _console.Write(new Panel("An error occured during execution").BorderColor(Color.Red).RoundedBorder());
                _console.WriteException(ex, ExceptionFormats.ShortenEverything);

                return -1;
            }
        }
    }
}
