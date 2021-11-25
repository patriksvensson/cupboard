using Cupboard;

namespace Sandbox;

public sealed class Rust : Manifest
{
    public override void Execute(ManifestContext context)
    {
        // Download directory
        context.Resource<Directory>("~/Downloads")
            .Ensure(DirectoryState.Present);

        if (context.Facts["rust.installed"])
        {
            return;
        }

        if (context.Facts.IsWindows())
        {
            // Download Rust installer
            context.Resource<Download>("https://win.rustup.rs/x86_64")
                .ToFile("~/Downloads/rustup-init.exe");

            // Run Rust installer
            context.Resource<Exec>("~/Downloads/rustup-init.exe")
                .Arguments("-y")
                .ValidExitCodes(0, 1)
                .After<Download>("https://win.rustup.rs/x86_64");
        }
        else
        {
            // Download Rust installer script
            context.Resource<Download>("https://sh.rustup.rs")
                .Permissions("700")
                .After<Directory>("~/Downloads")
                .ToFile("~/Downloads/rustup.sh");

            // Run Rust installer
            context.Resource<Exec>("~/Downloads/rustup.sh")
                .Arguments("-y")
                .ValidExitCodes(0, 1)
                .After<Download>("https://sh.rustup.rs");
        }
    }
}