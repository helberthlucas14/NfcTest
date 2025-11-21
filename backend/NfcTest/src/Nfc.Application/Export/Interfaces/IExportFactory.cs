namespace Nfc.Application.Export.Interfaces
{
    public interface IExportFactory
    {
        IExporter Create(ExportType type);
    }
}
