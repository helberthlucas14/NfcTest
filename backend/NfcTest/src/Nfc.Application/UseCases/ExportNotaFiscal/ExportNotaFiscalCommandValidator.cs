using FluentValidation;

namespace Nfc.Application.UseCases.ExportNotaFiscal
{
    public class ExportNotaFiscalCommandValidator
        : AbstractValidator<ExportNotaFiscalCommand>
    {
        public ExportNotaFiscalCommandValidator()
        {
            RuleFor(x => x.Type)
                .IsInEnum();

            RuleFor(x => x.Ids)
               .Must(collection => collection == null || !collection.Any())
               .WithMessage("Ids not be empty or null.");
        }
    }
}
