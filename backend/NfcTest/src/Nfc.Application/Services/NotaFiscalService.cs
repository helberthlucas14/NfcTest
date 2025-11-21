using Microsoft.Extensions.Logging;
using Nfc.Application.UseCases.NotaFiscal.Common;
using Nfc.Domain.Entity;
using Nfc.Domain.Interfaces.Repositories;
using Nfc.Domain.Interfaces.Services;

namespace Nfc.Application.Services
{
    public class NotaFiscalService : INotaFiscalService
    {
        private readonly INotaFiscalRepository _repository;
        private ILogger<NotaFiscalService> _logger;
        public NotaFiscalService(
            INotaFiscalRepository repository,
            ILogger<NotaFiscalService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public IQueryable<NotaFiscal> GetAllQuery => _repository.GetAllQuery;

        public async Task<bool> ExistsAsync(long id, CancellationToken cancellationToken)
            => await _repository.ExistsAsync(id, cancellationToken);

        public async Task<PagedList<NotaFiscal>> GetAllQueryAsync(NotaFiscalQueryStringParameters parameters, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Operation: {nameof(GetAllQueryAsync)} : Entity:{nameof(NotaFiscal)} : {parameters}  : {DateTime.Now}");
            return await _repository.GetAllAsync(parameters, cancellationToken);
        }

        public Task<NotaFiscal> GetByIdAsync(long id, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Operation: {nameof(GetByIdAsync)} : Entity:{nameof(NotaFiscal)} : {id}  : {DateTime.Now}");
            return _repository.GetByIdAsync(id, cancellationToken);
        }

        public async Task<NotaFiscal> RegisterAsync(NotaFiscal entity, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Operation: {nameof(RegisterAsync)} : Entity:{nameof(NotaFiscal)}  : {DateTime.Now}");
            return await _repository.AddAsync(entity, cancellationToken);
        }

        public async Task<NotaFiscal> UpdateAsync(
            long id,
            string emissor,
            DateTime dataEmissao,
            IList<Item> items,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Operation: {nameof(UpdateAsync)} : Entity:{nameof(NotaFiscal)} : {id} : {DateTime.Now}");
            var entity = await _repository.GetByIdAsync(id, cancellationToken);
            entity.Atualizar(emissor, dataEmissao);

            var existingById = entity.Items.ToDictionary(i => i.Id);
            entity.RemoverTodosItens();

            foreach (var item in items)
                entity.AdicionarItem(new Item(entity.Id, item.Descricao, item.Valor));

            await _repository.UpdateAsync(entity, cancellationToken);

            return entity;
        }

        public async Task UpdateAsync(NotaFiscal entity, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Operation: {nameof(UpdateAsync)} : Entity:{nameof(NotaFiscal)} : {entity.Id} : {DateTime.Now}");
            var getEntity = await _repository.GetByIdAsync(entity.Id, cancellationToken);
            await _repository.UpdateAsync(entity, cancellationToken);
        }

        public async Task DeleteByIdAsync(long id, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Operation: {nameof(DeleteByIdAsync)} : Entity:{nameof(NotaFiscal)} : {id} : {DateTime.Now}");
            var entity = await _repository.GetByIdAsync(id, cancellationToken);
            await _repository.DeleteByIdAsync(entity, cancellationToken);
        }
    }
}
