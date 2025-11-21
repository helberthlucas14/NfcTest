using FluentValidation;

namespace Nfc.Application.UseCases.NotaFiscal.UpdateNotaFiscal
{
    public class UpdateNotaFiscalValidator : AbstractValidator<UpdateNotaFiscalCommand>
    {
        public UpdateNotaFiscalValidator()
        {
            RuleFor(x => x.Emissor)
                .NotEmpty()
                .MinimumLength(2)
                .MaximumLength(150);
            
            RuleFor(x => x.DataEmissao)
                .LessThanOrEqualTo(DateTime.Now);

            RuleForEach(x => x.Items).
                SetValidator(new UpdateItemRequestValidator());
        }
    }
    public class UpdateItemRequestValidator : AbstractValidator<UpdateItemRequest>
    {
        public UpdateItemRequestValidator()
        {
            RuleFor(x => x.Descricao)
                .NotEmpty()
                .MinimumLength(3)
                .MaximumLength(255);

            RuleFor(x => x.Valor)
                .GreaterThan(0);
        }
    }
}
