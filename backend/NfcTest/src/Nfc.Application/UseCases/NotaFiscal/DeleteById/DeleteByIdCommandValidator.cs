using FluentValidation;

namespace Nfc.Application.UseCases.NotaFiscal.DeleteById
{
    public class DeleteByIdCommandValidator : AbstractValidator<DeleteByIdCommand>
    {
        public DeleteByIdCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0);
        }
    }
}
