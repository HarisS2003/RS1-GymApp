using eGymSystem.Application.Abstractions.Validation;
using eGymSystem.Shared.Dtos;
using eGymSystem.Shared.Errors;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace eGymSystem.Infrastructure.Common;

public sealed class eGymExceptionHandler(IHostEnvironment environment) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not CommandValidationException validationException)
        {
            return false;
        }

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        var dto = new ErrorDto
        {
            Code = ApiErrorCodes.ValidationFailed,
            Message = "Validation failed.",
            TraceId = httpContext.TraceIdentifier,
            Details = environment.IsDevelopment() ? string.Join("; ", validationException.Errors) : null
        };

        await httpContext.Response.WriteAsJsonAsync(dto, cancellationToken);
        return true;
    }
}
