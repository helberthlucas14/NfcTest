using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.Runtime;
using Microsoft.Extensions.Options;
using Nfc.Application.Export;
using Nfc.Application.Export.Interfaces;

namespace Nfc.Infra.Storage
{
    public class S3ExportFileStorage : IExportFileStorage
    {
        private readonly ExportFileStorageOptions _options;

        public S3ExportFileStorage(IOptions<ExportFileStorageOptions> options)
        {
            _options = options.Value;
        }

        private AmazonS3Client CreateClient()
        {
            var region = RegionEndpoint.GetBySystemName(_options.S3.Region);
            var config = new AmazonS3Config { RegionEndpoint = region };
            if (!string.IsNullOrWhiteSpace(_options.S3.ServiceUrl))
            {
                config.ServiceURL = _options.S3.ServiceUrl;
                config.ForcePathStyle = true; 
            }
            if (!string.IsNullOrWhiteSpace(_options.S3.AccessKey) && !string.IsNullOrWhiteSpace(_options.S3.SecretKey))
            {
                var creds = new BasicAWSCredentials(_options.S3.AccessKey, _options.S3.SecretKey);
                return new AmazonS3Client(creds, config);
            }
            return new AmazonS3Client(config);
        }

        public async Task<StoredFileInfo> SaveAsync(string jobId, ExportType type, Stream content, CancellationToken cancellationToken)
        {
            var client = CreateClient();
            var key = BuildObjectKey(jobId, type);
            var request = new PutObjectRequest
            {
                BucketName = _options.S3.Bucket,
                Key = key,
                InputStream = content,
                ContentType = MapContentType(type)
            };
            await client.PutObjectAsync(request, cancellationToken);
            var fileName = Path.GetFileName(key);
            return new StoredFileInfo(fileName, MapContentType(type), Stream.Null);
        }

        public async Task<StoredFileInfo?> OpenReadAsync(string jobId, ExportType type, CancellationToken cancellationToken)
        {
            var client = CreateClient();
            var key = BuildObjectKey(jobId, type);
            try
            {
                var response = await client.GetObjectAsync(_options.S3.Bucket, key, cancellationToken);
                var fileName = Path.GetFileName(key);
                return new StoredFileInfo(fileName, MapContentType(type), response.ResponseStream);
            }
            catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public Task<string?> GetPublicUrlAsync(string jobId, ExportType type, TimeSpan? expires = null, CancellationToken cancellationToken = default)
        {
            var client = CreateClient();
            var key = BuildObjectKey(jobId, type);
            var expirySeconds = expires?.TotalSeconds is > 0
                ? (int)expires.Value.TotalSeconds
                : _options.S3.PresignExpirySeconds ?? _options.S3.PresignExpiryMinutes * 60;
            var req = new GetPreSignedUrlRequest
            {
                BucketName = _options.S3.Bucket,
                Key = key,
                Expires = DateTime.UtcNow.AddSeconds(expirySeconds)
            };
            var url = client.GetPreSignedURL(req);
            return Task.FromResult<string?>(url);
        }

        private string BuildObjectKey(string jobId, ExportType type)
        {
            var ext = GetExtension(type);
            var prefix = _options.S3.KeyPrefix?.Trim('/') ?? "exports";
            return $"{prefix}/{jobId}{ext}";
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