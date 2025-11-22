using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using Microsoft.Extensions.Options;
using Nfc.Application.Export;
using Nfc.Application.Export.Storage;

namespace Nfc.Infra.Storage
{
    public class S3ExportFileStorage : IExportFileStorage
    {
        private readonly S3Options _options;
        private readonly IAmazonS3 _s3;

        public S3ExportFileStorage(IOptions<ExportFileStorageOptions> options)
        {
            _options = options.Value.S3 ?? new S3Options();

            var cfg = new AmazonS3Config
            {
                RegionEndpoint = !string.IsNullOrWhiteSpace(_options.Region) ? RegionEndpoint.GetBySystemName(_options.Region) : RegionEndpoint.USEast1,
                ForcePathStyle = _options.ForcePathStyle
            };
            if (!string.IsNullOrWhiteSpace(_options.ServiceUrl))
            {
                cfg.ServiceURL = _options.ServiceUrl;
            }

            _s3 = new AmazonS3Client(_options.AccessKey, _options.SecretKey, cfg);
        }

        public async Task<StoredFileInfo> SaveAsync(string jobId, ExportType type, byte[] content, CancellationToken cancellationToken)
        {
            var ext = GetExtension(type);
            var contentType = GetContentType(type);
            var objectKey = $"exports/{jobId}.{ext}";

            using var ms = new MemoryStream(content);
            var put = new PutObjectRequest
            {
                BucketName = _options.Bucket,
                Key = objectKey,
                InputStream = ms,
                ContentType = contentType
            };
            var resp = await _s3.PutObjectAsync(put, cancellationToken);

            string? url = await GetPublicUrlAsync(jobId, type, cancellationToken);
            return new StoredFileInfo(objectKey, contentType, content.LongLength, url);
        }

        public async Task<(Stream Stream, string ContentType)?> OpenReadAsync(string jobId, ExportType type, CancellationToken cancellationToken)
        {
            var ext = GetExtension(type);
            var contentType = GetContentType(type);
            var objectKey = $"exports/{jobId}.{ext}";

            try
            {
                var resp = await _s3.GetObjectAsync(_options.Bucket, objectKey, cancellationToken);
                return (resp.ResponseStream, resp.Headers.ContentType ?? contentType);
            }
            catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public Task<string?> GetPublicUrlAsync(string jobId, ExportType type, CancellationToken cancellationToken)
        {
            var ext = GetExtension(type);
            var objectKey = $"exports/{jobId}.{ext}";
            var req = new GetPreSignedUrlRequest
            {
                BucketName = _options.Bucket,
                Key = objectKey,
                Expires = DateTime.UtcNow.AddMinutes(_options.PresignExpiryMinutes)
            };
            var presigned = _s3.GetPreSignedURL(req);
            return Task.FromResult<string?>(presigned);
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