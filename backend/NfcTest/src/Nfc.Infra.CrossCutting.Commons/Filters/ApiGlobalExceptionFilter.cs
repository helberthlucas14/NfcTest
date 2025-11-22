using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Nfc.Application.Exceptions;
using Nfc.Domain.Exceptions;

namespace Nfc.Infra.CrossCutting.Commons.Filters
{
    public class ApiGlobalExceptionFilter : IExceptionFilter
    {
        private readonly IHostEnvironment _env;
        public ApiGlobalExceptionFilter(IHostEnvironment env)
            => _env = env;

        public void OnException(ExceptionContext context)
        {
            var details = new ProblemDetails();
            var exception = context.Exception;

            if (_env.IsDevelopment())
                details.Extensions.Add("StackTrace", exception.StackTrace);

            if (exception is EntityValidationException validationEx)
            {
                details.Title = "One or more validation errors ocurred";
                details.Status = StatusCodes.Status422UnprocessableEntity;
                details.Type = "UnprocessableEntity";
                details.Detail = exception!.Message;
                if (validationEx.Errors is not null)
                    details.Extensions["Errors"] = validationEx.Errors.Select(e => e.Message).ToArray();
            }
            else if (exception is NotFoundException)
            {
                details.Title = "Not Found";
                details.Status = StatusCodes.Status404NotFound;
                details.Type = "NotFound";
                details.Detail = exception!.Message;
            }
            else
            {
                details.Title = "An unexpected error ocurred";
                details.Status = StatusCodes.Status422UnprocessableEntity;
                details.Type = "UnexpectedError";
                details.Detail = exception.Message;
            }

            context.HttpContext.Response.StatusCode = (int)details.Status;
            context.Result = new ObjectResult(details);
            context.ExceptionHandled = true;
        }
    }

}
