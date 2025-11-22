using FluentValidation;

namespace Nfc.Application.UseCases.Export.GetExportStatusByJobId
{
    public class GetExportStatusByJobIdQueryValidator : AbstractValidator<GetExportStatusByJobIdQuery>
    {
        public GetExportStatusByJobIdQueryValidator()
        {
            RuleFor(x => x.JobId)
                .NotNull()
                .NotEmpty();
        }
    }
}
