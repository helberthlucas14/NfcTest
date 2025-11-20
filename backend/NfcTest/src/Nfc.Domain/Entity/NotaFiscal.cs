using EntyCore = Nfc.Domain.Core.Models;

namespace Nfc.Domain.Entity
{
    public class NotaFiscal : EntyCore.Entity
    {
        public string Emissor { get; private set; }
        public DateTime DataEmissao { get; private set; }
        public List<Item> Items { get; private set; } = new();
        public decimal ValoTotal => CalcularValorTotal();
        public NotaFiscal(string emissor, DateTime dataEmissao)
        {
            Emissor = emissor;
            DataEmissao = dataEmissao;
            Validar();
        }
        public virtual decimal CalcularValorTotal()
            => Items.Sum(i => i.Valor);

        public virtual void AdicionarItem(Item item)
        {
            Items.Add(item);
            Validar();
        }

        public void RemoverItem(Item item)
        {
            Items.Remove(item);
            Validar();
        }

        public void RemoverTodosItens()
        {
            Items.Clear();
            Validar();
        }

        public void Atualizar(string? emissor = null, DateTime? dataEmissao = null)
        {
            Emissor = emissor ?? Emissor;
            DataEmissao = dataEmissao ?? DataEmissao;
            Validar();
        }

        private void Validar()
        {
            Validation.DomainValidation.NotNullOrEmpty(Emissor, nameof(Emissor));
            Validation.DomainValidation.MinLength(Emissor, 2, nameof(Emissor));
            Validation.DomainValidation.MaxLength(Emissor, 150, nameof(Emissor));
            Validation.DomainValidation.NotNull(DataEmissao, nameof(DataEmissao));
        }

    }
}
