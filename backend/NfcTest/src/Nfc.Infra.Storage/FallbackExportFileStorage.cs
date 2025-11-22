using Nfc.Application.Export;
using Nfc.Application.Export.Interfaces;

namespace Nfc.Infra.Storage
{
    public class FallbackExportFileStorage : IExportFileStorage
    {
        private readonly IExportFileStorage _primary;
        private readonly IExportFileStorage _fallback;

        public FallbackExportFileStorage(IExportFileStorage primary, IExportFileStorage fallback)
        {
            _primary = primary;
            _fallback = fallback;
        }

        public async Task<StoredFileInfo> SaveAsync(string jobId, ExportType type, Stream content, CancellationToken cancellationToken)
        {
            using var buffer = new MemoryStream();
            await content.CopyToAsync(buffer, cancellationToken);
            var bytes = buffer.ToArray();
            try
            {
                using var msPrimary = new MemoryStream(bytes);
                return await _primary.SaveAsync(jobId, type, msPrimary, cancellationToken);
            }
            catch
            {
                using var msFallback = new MemoryStream(bytes);
                return await _fallback.SaveAsync(jobId, type, msFallback, cancellationToken);
            }
        }

        public async Task<StoredFileInfo?> OpenReadAsync(string jobId, ExportType type, CancellationToken cancellationToken)
        {
            try
            {
                var primary = await _primary.OpenReadAsync(jobId, type, cancellationToken);
                if (primary is not null)
                {
                    return primary;
                }
            }
            catch { }

            return await _fallback.OpenReadAsync(jobId, type, cancellationToken);
        }

        public async Task<string?> GetPublicUrlAsync(string jobId, ExportType type, TimeSpan? expires = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var url = await _primary.GetPublicUrlAsync(jobId, type, expires, cancellationToken);
                if (!string.IsNullOrWhiteSpace(url))
                {
                    return url;
                }
            }
            catch { }

            return await _fallback.GetPublicUrlAsync(jobId, type, expires, cancellationToken);
        }
    }
}