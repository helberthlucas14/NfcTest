using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Nfc.Application.Export;
using Nfc.Application.Export.Interfaces;

namespace Nfc.Infra.Storage
{
    public class LocalExportFileStorage : IExportFileStorage
    {
        private readonly string _rootDirectory;

        public LocalExportFileStorage(IOptions<ExportFileStorageOptions> options, IHostEnvironment env)
        {
            var local = options.Value.Local;
            var dir = local.Directory ?? "data/tmp/exports";
            if (!Path.IsPathRooted(dir))
            {
                dir = Path.GetFullPath(Path.Combine(env.ContentRootPath, dir));
            }
            Directory.CreateDirectory(dir);
            _rootDirectory = dir;
        }

        public async Task<StoredFileInfo> SaveAsync(string jobId, ExportType type, Stream content, CancellationToken cancellationToken)
        {
            var fileName = BuildFileName(jobId, type);
            var filePath = Path.Combine(_rootDirectory, fileName);
            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await content.CopyToAsync(fs, cancellationToken);
            }
            return new StoredFileInfo(fileName, MapContentType(type), Stream.Null);
        }

        public Task<StoredFileInfo?> OpenReadAsync(string jobId, ExportType type, CancellationToken cancellationToken)
        {
            var fileName = BuildFileName(jobId, type);
            var filePath = Path.Combine(_rootDirectory, fileName);
            if (!File.Exists(filePath))
            {
                return Task.FromResult<StoredFileInfo?>(null);
            }
            var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            return Task.FromResult<StoredFileInfo?>(new StoredFileInfo(fileName, MapContentType(type), stream));
        }

        public Task<string?> GetPublicUrlAsync(string jobId, ExportType type, TimeSpan? expires = null, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<string?>(null);
        }

        private static string BuildFileName(string jobId, ExportType type)
        {
            return $"{jobId}{GetExtension(type)}";
        }

        private static string MapContentType(ExportType type)
        {
            return type switch
            {
                ExportType.JSON => "application/json",
                ExportType.TXT => "text/plain",
                _ => "application/octet-stream"
            };
        }

        private static string GetExtension(ExportType type)
        {
            return type switch
            {
                ExportType.JSON => ".json",
                ExportType.TXT => ".txt",
                _ => ".bin"
            };
        }
    }
}