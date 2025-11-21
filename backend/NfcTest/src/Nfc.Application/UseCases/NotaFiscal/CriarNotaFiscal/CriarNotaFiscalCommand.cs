using Nfc.Application.UseCases.Base;

namespace Nfc.Application.UseCases.NotaFiscal.CriarNotaFiscal
{
    public class CriarNotaFiscalCommand : CommandRequestBase<Common.NotaFiscalResponse>
    {
        public string Emissor { get; set; }
        public DateTime DataEmissao { get; set; }
        public List<ItemModelRequest> Items { get; set; } = new();
    }
}
