using Nfc.Application.Export;
using Nfc.Application.UseCases.Base;

namespace Nfc.Application.UseCases.Export.ExportNotaFiscal
{
    public class ExportNotaFiscalCommand : CommandRequestBase<string>
    {
        public ExportType Type { get; set; }
        public long[] Ids { get; set; } = [];
    }
}
