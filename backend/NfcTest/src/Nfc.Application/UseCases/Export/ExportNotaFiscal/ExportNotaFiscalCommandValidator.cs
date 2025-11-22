using FluentValidation;

namespace Nfc.Application.UseCases.Export.ExportNotaFiscal
{
    public class ExportNotaFiscalCommandValidator
        : AbstractValidator<ExportNotaFiscalCommand>
    {
        public ExportNotaFiscalCommandValidator()
        {
            RuleFor(x => x.Type)
                .IsInEnum();

            RuleFor(x => x.Ids)
                .NotNull()
                .Must(collection => collection.Any())
                .WithMessage("Ids not be null or empty;")
                .Must(collection => collection.All(id => id > 0))
                .WithMessage("Ids should be greater than 0.");
        }
    }
}
