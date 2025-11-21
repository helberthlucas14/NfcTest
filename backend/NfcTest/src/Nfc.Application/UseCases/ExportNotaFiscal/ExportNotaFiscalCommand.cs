using Nfc.Application.Export;
using Nfc.Application.UseCases.Base;

namespace Nfc.Application.UseCases.ExportNotaFiscal
{
    public class ExportNotaFiscalCommand : CommandRequestBase<long>
    {
        public ExportType Type { get; set; }
        public long[] Ids { get; set; } = [];
    }
}
