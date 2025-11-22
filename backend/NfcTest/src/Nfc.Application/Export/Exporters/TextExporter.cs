using Nfc.Application.Export.Interfaces;
using Nfc.Domain.Entity;
using System.Text;

namespace Nfc.Application.Export.Exporters
{

    public class TextExporter : IExporter
    {
        public ExportType Type => ExportType.TXT;

        public Task<byte[]> ExportAsync<T>(IEnumerable<T> data, CancellationToken cancellationToken)
        {
            var sb = new StringBuilder();

            if (data is IEnumerable<NotaFiscal> notas)
            {
                foreach (var n in notas)
                {

                    sb.AppendLine($"Nota {n.Id}");
                    sb.AppendLine($"Emissor: {n.Emissor}");
                    sb.AppendLine($"Data de Emissão: {n.DataEmissao:yyyy-MM-dd}");
                    sb.AppendLine($"Valor Total: {n.ValoTotal:0.###}");
                    sb.AppendLine("Itens:");
                    foreach (var i in n.Items)
                    {
                        sb.AppendLine($"  - {i.Descricao} | Valor: {i.Valor:0.###}");
                    }
                    sb.AppendLine(new string('-', 40));
                    Thread.Sleep(TimeSpan.FromSeconds(10));
                }
            }
            else
            {
                foreach (var item in data ?? Enumerable.Empty<T>())
                {
                    sb.AppendLine(item?.ToString() ?? string.Empty);
                }
            }

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            return Task.FromResult(bytes);
        }
    }
}
