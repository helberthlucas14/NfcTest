using Microsoft.Extensions.Logging;
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

        public async Task UpdateAsync(NotaFiscal entity, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Operation: {nameof(UpdateAsync)} : Entity:{nameof(NotaFiscal)} : {entity.Id} : {DateTime.Now}");
            var existingEntity = await _repository.GetByIdAsync(entity.Id, cancellationToken);
            await _repository.UpdateAsync(entity, cancellationToken);
        }
    }
}
