using Amazon;

namespace Nfc.Infra.Storage
{
    public class ExportFileStorageOptions
    {
        public string Provider { get; set; } = "S3"; 
        public LocalOptions Local { get; set; } = new();
        public S3Options S3 { get; set; } = new();
    }

    public class LocalOptions
    {
        public string Directory { get; set; } = "data/tmp/exports";
        public string? BaseUrl { get; set; }
    }

    public class S3Options
    {
        public string Bucket { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public string? AccessKey { get; set; }
        public string? SecretKey { get; set; }
        public string? ServiceUrl { get; set; }
        public string? BaseUrl { get; set; }
        public int PresignExpiryMinutes { get; set; } = 60;
        public int? PresignExpirySeconds { get; set; }
        public string KeyPrefix { get; set; } = "exports";
    }
}