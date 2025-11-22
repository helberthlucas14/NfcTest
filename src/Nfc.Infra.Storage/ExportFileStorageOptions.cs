using System;

namespace Nfc.Infra.Storage
{
    public class ExportFileStorageOptions
    {
        public string Provider { get; set; } = "Local";
        public LocalOptions Local { get; set; } = new();
        public S3Options S3 { get; set; } = new();
    }

    public class LocalOptions
    {
        public string Directory { get; set; } = "exports";
        public string? BaseUrl { get; set; }
    }

    public class S3Options
    {
        public string Bucket { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public string? BaseUrl { get; set; }
        public string? ServiceUrl { get; set; }
        public string AccessKey { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
        public int PresignExpiryMinutes { get; set; } = 60;
        public bool ForcePathStyle { get; set; } = true;
    }
}