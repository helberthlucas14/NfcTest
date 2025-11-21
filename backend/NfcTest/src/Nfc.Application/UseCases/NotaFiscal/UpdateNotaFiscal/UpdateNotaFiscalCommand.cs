using Nfc.Application.UseCases.Base;
using Nfc.Application.UseCases.NotaFiscal.Common;

namespace Nfc.Application.UseCases.NotaFiscal.UpdateNotaFiscal
{
    public class UpdateNotaFiscalCommand : CommandRequestBase<NotaFiscalResponse>
    {
        public long Id { get; set; }
        public string Emissor { get; set; }
        public DateTime DataEmissao { get; set; }
        public List<UpdateItemRequest> Items { get; set; } = new();
    }


    public class UpdateItemRequest
    {
        public string Descricao { get; set; }
        public decimal Valor { get; set; }
    }
}
