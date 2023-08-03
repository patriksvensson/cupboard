using System;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Spectre.IO;

namespace Cupboard;

public sealed class DownloadProvider : AsyncResourceProvider<Download>
{
    private readonly ICupboardFileSystem _fileSystem;
    private readonly ICupboardEnvironment _environment;
    private readonly ICupboardLogger _logger;
    private readonly HttpClient _http;

    private static readonly Regex _contentDispositionRegex = new("filename=\"(?'filename'.*)\"");

    public DownloadProvider(
        ICupboardFileSystem fileSystem,
        ICupboardEnvironment environment,
        ICupboardLogger logger)
    {
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _http = new HttpClient();
    }

    public override Download Create(string name)
    {
        return new Download(name);
    }

    public override async Task<ResourceState> RunAsync(IExecutionContext context, Download resource)
    {
        if (resource.Url == null)
        {
            _logger.Error($"Can't download file '{resource.Name}' since no URL has been set");
            return ResourceState.Error;
        }

        if (resource.Destination == null)
        {
            _logger.Error($"Can't download file from {resource.Url.AbsoluteUri} since destination has not been set");
            return ResourceState.Error;
        }

        if (context.DryRun)
        {
            return ResourceState.Unknown;
        }

        using (var request = new HttpRequestMessage(HttpMethod.Get, resource.Url))
        {
            using (var response = await _http.SendAsync(request).ConfigureAwait(false))
            {
                string? filename = null;
                if (response.Headers.TryGetValues("Content-Disposition", out var filenames))
                {
                    var match = _contentDispositionRegex.Match(filenames.First());
                    if (match.Success && match.Groups.ContainsKey("filename"))
                    {
                        filename = match.Groups["filename"].Value;
                    }
                }

                var path = GetDestinationPath(resource.Destination, filename);
                if (path == null)
                {
                    return ResourceState.Error;
                }

                if (_fileSystem.Exist(path))
                {
                    _logger.Debug($"File has already been downloaded from {resource.Url.AbsoluteUri}");
                    return ResourceState.Unchanged;
                }

                _logger.Debug($"Downloading file from {resource.Url.AbsoluteUri}");
                using (var inputStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                using (var outputStream = _fileSystem.GetFile(path).OpenWrite())
                {
                    await inputStream.CopyToAsync(outputStream).ConfigureAwait(false);
                }

                if (resource.Permissions != null)
                {
                    path.SetPermissions(resource.Permissions);
                }

                return ResourceState.Changed;
            }
        }
    }

    private FilePath? GetDestinationPath(Path? path, string? filename)
    {
        if (path != null)
        {
            if (path is DirectoryPath directory)
            {
                if (filename == null)
                {
                    _logger.Error("Could not resolve filename from download URL");
                    return null;
                }

                return directory
                    .MakeAbsolute(_environment)
                    .CombineWithFilePath(filename);
            }
            else if (path is FilePath file)
            {
                return file.MakeAbsolute(_environment);
            }
        }

        _logger.Error("Unknown destination type");
        return null;
    }
}
