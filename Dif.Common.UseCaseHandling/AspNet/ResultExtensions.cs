using Microsoft.AspNetCore.Http;
using Dif.Common.ResultExtensions;
using Dif.Common.ResultExtensions.Errors;

namespace Dif.Common.UseCaseHandling.AspNet;

using FluentResults;
using IResult = Microsoft.AspNetCore.Http.IResult;

public static class ResultExtensions
{
    public static IResult ToHttpResult<T>(this Result<T> result)
    {
        if (result.IsSuccess)
        {
            return Microsoft.AspNetCore.Http.Results.Ok(result.Value);
        }

        var firstError = result.Errors.FirstOrDefault();

        return firstError switch
        {
            ValidationError => Microsoft.AspNetCore.Http.Results.BadRequest(new
            {
                Errors = result.ToErrorDictionary()
            }),

            NotFoundError notFound => Microsoft.AspNetCore.Http.Results.NotFound(new
            {
                Message = notFound.Message,
                ResourceType = notFound.ResourceType,
                ResourceId = notFound.ResourceId
            }),

            ConflictError conflict => Microsoft.AspNetCore.Http.Results.Conflict(new
            {
                Message = conflict.Message,
                ResourceId = conflict.ResourceId
            }),

            _ when firstError?.Metadata.ContainsKey("ErrorType") == true
                   && firstError.Metadata["ErrorType"]?.ToString() == "Unauthorized"
                => Microsoft.AspNetCore.Http.Results.Unauthorized(),

            _ => Microsoft.AspNetCore.Http.Results.Problem(
                detail: result.GetErrorMessage(),
                statusCode: StatusCodes.Status500InternalServerError)
        };
    }

    public static IResult ToHttpResult(this Result result)
    {
        if (result.IsSuccess)
        {
            return Microsoft.AspNetCore.Http.Results.NoContent();
        }

        var firstError = result.Errors.FirstOrDefault();

        return firstError switch
        {
            ValidationError => Microsoft.AspNetCore.Http.Results.BadRequest(new
            {
                Errors = result.ToErrorDictionary()
            }),

            NotFoundError notFound => Microsoft.AspNetCore.Http.Results.NotFound(new
            {
                Message = notFound.Message,
                ResourceType = notFound.ResourceType,
                ResourceId = notFound.ResourceId
            }),

            ConflictError conflict => Microsoft.AspNetCore.Http.Results.Conflict(new
            {
                Message = conflict.Message,
                ResourceId = conflict.ResourceId
            }),

            _ when firstError?.Metadata.ContainsKey("ErrorType") == true
                   && firstError.Metadata["ErrorType"]?.ToString() == "Unauthorized"
                => Microsoft.AspNetCore.Http.Results.Unauthorized(),

            _ => Microsoft.AspNetCore.Http.Results.Problem(
                detail: result.GetErrorMessage(),
                statusCode: StatusCodes.Status500InternalServerError)
        };
    }
}
