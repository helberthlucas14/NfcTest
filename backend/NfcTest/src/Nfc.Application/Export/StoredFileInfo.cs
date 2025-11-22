namespace Nfc.Application.Export
{
    public record StoredFileInfo(string FileName, string ContentType, Stream ContentStream);
}