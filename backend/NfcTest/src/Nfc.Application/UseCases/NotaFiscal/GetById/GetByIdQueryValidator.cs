using FluentValidation;

namespace Nfc.Application.UseCases.NotaFiscal.GetById
{
    public class GetByIdQueryValidator : AbstractValidator<GetByIdQuery>
    {
        public GetByIdQueryValidator()
             => RuleFor(x => x.Id).NotEmpty();
    }
}
