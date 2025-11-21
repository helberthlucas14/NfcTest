using DomainEntity = Nfc.Domain.Entity;
namespace Nfc.Application.UseCases.NotaFiscal.Common
{
    public class NotaFiscalResponse
    {
        public long Id { get; set; }
        public string Emissor { get; set; }
        public DateTime DataEmissao { get; set; }
        public decimal ValoTotal { get; set; }
        public List<ItemModelResponse> Items { get; set; } = new();

        public static NotaFiscalResponse FromMember(DomainEntity.NotaFiscal notaFiscal)
        {
            return new NotaFiscalResponse
            {
                Id = notaFiscal.Id,
                Emissor = notaFiscal.Emissor,
                DataEmissao = notaFiscal.DataEmissao,
                ValoTotal = notaFiscal.ValoTotal,
                Items = notaFiscal.Items.Select(item => new ItemModelResponse
                {
                    NotaFiscalId = item.NotaFiscalId,
                    Descricao = item.Descricao,
                    Valor = item.Valor
                }).ToList()
            };
        }
    }

    public class ItemModelResponse
    {
        public long NotaFiscalId { get; set; }
        public string Descricao { get; set; }
        public decimal Valor { get; set; }
    }
}
