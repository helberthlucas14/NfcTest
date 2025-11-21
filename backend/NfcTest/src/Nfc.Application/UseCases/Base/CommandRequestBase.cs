
using MediatR;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Nfc.Application.UseCases.Base
{
    public abstract class CommandRequestBase<TResponse> : IRequest<TResponse>
    {
        [NotMapped]
        [JsonIgnore]
        public Guid CorrelationId { get; set; } = Guid.Empty;

        [NotMapped]
        [JsonIgnore]
        public string? JobId { get; set; }
    }
}
