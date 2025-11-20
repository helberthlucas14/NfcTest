using EntyCore = Nfc.Domain.Core.Models;

namespace Nfc.Domain.Entity
{
    public class Item : EntyCore.Entity
    {
        public Guid NotaFiscalId { get; private set; }
        public string Descricao { get; private set; }
        public decimal Valor { get; private set; }

        public Item(Guid notaFiscalId, string descricao, decimal valor)
        {
            NotaFiscalId = notaFiscalId;
            Descricao = descricao;
            Valor = valor;
        }

        public void Atualizar(
            string? descricao = null,
            decimal? valor = null)
        {
            Descricao = descricao ?? Descricao;
            Valor = valor ?? Valor;
            Validar();
        }

        private void Validar()
        {
            Validation.DomainValidation.NotNullOrEmpty(Descricao, nameof(Descricao));
            Validation.DomainValidation.MinLength(Descricao, 3, nameof(Descricao));
            Validation.DomainValidation.MaxLength(Descricao, 255, nameof(Descricao));

            Validation.DomainValidation.InvalidAtributeMinValue(Valor, nameof(Valor));
            Validation.DomainValidation.NotNull(Valor, nameof(Valor));
        }
    }
}
