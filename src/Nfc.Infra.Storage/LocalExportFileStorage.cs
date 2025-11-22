using Microsoft.Extensions.Options;
using Nfc.Application.Export;
using Nfc.Application.Export.Storage;

namespace Nfc.Infra.Storage
{
    public class LocalExportFileStorage : IExportFileStorage
    {
        private readonly LocalOptions _options;
        private readonly string _rootDir;

        public LocalExportFileStorage(IOptions<ExportFileStorageOptions> options)
        {
            _options = options.Value.Local ?? new LocalOptions();
            _rootDir = Path.GetFullPath(_options.Directory, Directory.GetCurrentDirectory());
            Directory.CreateDirectory(_rootDir);
        }

        public Task<StoredFileInfo> SaveAsync(string jobId, ExportType type, byte[] content, CancellationToken cancellationToken)
        {
            var ext = GetExtension(type);
            var contentType = GetContentType(type);
            var fileName = $"{jobId}.{ext}";
            var fullPath = Path.Combine(_rootDir, fileName);

            File.WriteAllBytes(fullPath, content);

            var fileUrl = $"/api/export/file/{jobId}";
            var info = new StoredFileInfo(fileName, contentType, content.LongLength, fileUrl);
            return Task.FromResult(info);
        }

        public Task<(Stream Stream, string ContentType)?> OpenReadAsync(string jobId, ExportType type, CancellationToken cancellationToken)
        {
            var ext = GetExtension(type);
            var contentType = GetContentType(type);
            var fileName = $"{jobId}.{ext}";
            var fullPath = Path.Combine(_rootDir, fileName);
            if (!File.Exists(fullPath))
                return Task.FromResult<(Stream, string)?>(null);

            var stream = File.OpenRead(fullPath);
            return Task.FromResult<(Stream, string)?>((stream, contentType));
        }

        public Task<string?> GetPublicUrlAsync(string jobId, ExportType type, CancellationToken cancellationToken)
        {
            return Task.FromResult<string?>(
                $"/api/export/file/{jobId}"
            );
        }

        private static string GetExtension(ExportType type) => type switch
        {
            ExportType.JSON => "json",
            ExportType.TXT => "txt",
            _ => "bin"
        };

        private static string GetContentType(ExportType type) => type switch
        {
            ExportType.JSON => "application/json",
            ExportType.TXT => "text/plain",
            _ => "application/octet-stream"
        };
    }
}