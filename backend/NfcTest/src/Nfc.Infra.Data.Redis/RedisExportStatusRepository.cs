using FC.Codeflix.Catalog.Application.Exceptions;
using Nfc.Application.Export;
using StackExchange.Redis;
using System.Text.Json;

namespace Nfc.Infra.Data.Redis
{
    public class RedisExportStatusRepository : IExportStatusRepository
    {
        private readonly IConnectionMultiplexer _connection;

        public RedisExportStatusRepository(IConnectionMultiplexer connection)
        {
            _connection = connection;
        }

        public async Task SaveAsync(ExportStatus status, CancellationToken cancellationToken)
        {
            var db = _connection.GetDatabase();
            var key = $"export:status:{status.JobId}";
            var json = JsonSerializer.Serialize(status);
            await db.StringSetAsync(key, json);
        }

        public async Task<ExportStatus?> GetAsync(string jobId, CancellationToken cancellationToken)
        {
            var db = _connection.GetDatabase();
            var key = $"export:status:{jobId}";
            var value = await db.StringGetAsync(key);

            NotFoundException.ThrowIfCondition(!value.HasValue, $"NotaFiscal '{jobId}' not found.");
            return JsonSerializer.Deserialize<ExportStatus>(value.ToString());
        }
    }
}