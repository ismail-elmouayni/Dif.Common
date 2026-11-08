namespace Dif.Common.ResultExtensions;

using FluentResults;

using Dif.Common.ResultExtensions.Errors;

/// <summary>
/// Fournit les méthodes utilitaires et les extensions utilisées
/// pour construire, enrichir et exploiter les résultats FluentResults.
/// </summary>
public static class ResultExtensions
{
    private const string GeneralErrorKey = "General";
    private const string IsWarningMetadataKey = "IsWarning";
    private const string WarningMessagesMetadataKey = "WarningMessages";

    #region SUCCESSES    

    public static Result Success()
        => Result.Ok();

    public static Result<T> Success<T>(T value)
        => Result.Ok(value);

    #endregion

    #region FAILURES    

    public static Result Failure(string message)
        => Result.Fail(message);

    public static Result Failure(IError error)
        => Result.Fail(error);

    public static Result Failure(IEnumerable<IError> errors)
    {
        var errorList = MaterializeRequired(
            errors,
            nameof(errors),
            "La collection d’erreurs ne peut pas être vide.");
        return Result.Fail(errorList);
    }

    public static Result Failure(IEnumerable<string> messages)
    {
        var messageList = MaterializeRequired(
            messages,
            nameof(messages),
            "La collection de messages d’erreur ne peut pas être vide.");
        return Result.Fail(messageList);
    }

    public static Result<T> Failure<T>(string message)
        => Result.Fail<T>(message);

    public static Result<T> Failure<T>(IError error)
        => Result.Fail<T>(error);

    public static Result<T> Failure<T>(
        IEnumerable<IError> errors)
    {
        var errorList = MaterializeRequired(
            errors,
            nameof(errors),
            "La collection d’erreurs ne peut pas être vide.");
        return Result.Fail<T>(errorList);
    }

    public static Result<T> Failure<T>(
        IEnumerable<string> messages)
    {
        var messageList = MaterializeRequired(
            messages,
            nameof(messages),
            "La collection de messages d’erreur ne peut pas être vide.");
        return Result.Fail<T>(messageList);
    }

    #endregion

    #region VALIDATION FAILURES    

    public static Result ValidationFailed(
        string message,
        string? propertyName = null)
    {
        return Result.Fail(
            new ValidationError(
                message,
                propertyName));
    }

    public static Result ValidationFailed(
        params (string message, string? propertyName)[] errors)
    {
        var validationErrors = CreateValidationErrors(errors);
        return Result.Fail(validationErrors);
    }

    public static Result ValidationFailed(
        IEnumerable<string> messages)
    {
        var messageList = MaterializeRequired(
            messages,
            nameof(messages),
            "Au moins un message de validation est requis.");
        var validationErrors = messageList
            .Select(
                message => (IError)new ValidationError(
                    message))
            .ToList();
        return Result.Fail(validationErrors);
    }

    public static Result<T> ValidationFailed<T>(
        string message,
        string? propertyName = null)
    {
        return Result.Fail<T>(
            new ValidationError(
                message,
                propertyName));
    }

    public static Result<T> ValidationFailed<T>(
        params (string message, string? propertyName)[] errors)
    {
        var validationErrors = CreateValidationErrors(errors);
        return Result.Fail<T>(validationErrors);
    }

    public static Result<T> ValidationFailed<T>(
        IEnumerable<string> messages)
    {
        var messageList = MaterializeRequired(
            messages,
            nameof(messages),
            "Au moins un message de validation est requis.");
        var validationErrors = messageList
            .Select(
                message => (IError)new ValidationError(
                    message))
            .ToList();
        return Result.Fail<T>(validationErrors);
    }

    public static Dictionary<string, List<string>>
        GetValidationErrors(
            this IResultBase result)
    {
        ArgumentNullException.ThrowIfNull(result);
        var validationErrors =
            new Dictionary<string, List<string>>(
                StringComparer.Ordinal);
        foreach ( var validationError in
                 result.Errors.OfType<ValidationError>() )
        {
            var propertyName =
                validationError.PropertyName
                ?? GeneralErrorKey;
            if ( !validationErrors.TryGetValue(
                    propertyName,
                    out var messages) )
            {
                messages = [];
                validationErrors[propertyName] = messages;
            }
            messages.Add(validationError.Message);
        }
        return validationErrors;
    }

    #endregion

    #region CONFLICT FAILURES    

    public static Result Conflict(
        string message,
        string? resourceId = null)
    {
        return Result.Fail(
            new ConflictError(
                message,
                resourceId));
    }

    public static Result<T> Conflict<T>(
        string message,
        string? resourceId = null)
    {
        return Result.Fail<T>(
            new ConflictError(
                message,
                resourceId));
    }

    public static ConflictError? GetConflictError(
        this IResultBase result)
    {
        ArgumentNullException.ThrowIfNull(result);
        return result.Errors
            .OfType<ConflictError>()
            .FirstOrDefault();
    }

    #endregion

    #region NOT FOUND FAILURES    

    public static Result NotFound(
        string message,
        string? resourceType = null,
        string? resourceId = null)
    {
        return Result.Fail(
            new NotFoundError(
                message,
                resourceType,
                resourceId));
    }

    public static Result<T> NotFound<T>(
        string message,
        string? resourceType = null,
        string? resourceId = null)
    {
        return Result.Fail<T>(
            new NotFoundError(
                message,
                resourceType,
                resourceId));
    }

    public static NotFoundError? GetNotFoundError(
        this IResultBase result)
    {
        ArgumentNullException.ThrowIfNull(result);
        return result.Errors
            .OfType<NotFoundError>()
            .FirstOrDefault();
    }

    #endregion    

    #region WARNING    

    public static Result WithWarning(
        this Result result,
        string message)
    {
        ArgumentNullException.ThrowIfNull(result);
        return result.WithSuccess(
            new Warning(message));
    }

    public static Result<T> WithWarning<T>(
        this Result<T> result,
        string message)
    {
        ArgumentNullException.ThrowIfNull(result);
        return result.WithSuccess(
            new Warning(message));
    }

    public static Result WithWarnings(
        this Result result,
        params string[] messages)
    {
        return result.WithWarnings(
            (IEnumerable<string>)messages);
    }

    public static Result WithWarnings(
        this Result result,
        IEnumerable<string> messages)
    {
        ArgumentNullException.ThrowIfNull(result);
        ArgumentNullException.ThrowIfNull(messages);
        foreach ( var message in messages )
        {
            result.WithSuccess(
                new Warning(message));
        }
        return result;
    }

    public static Result<T> WithWarnings<T>(
        this Result<T> result,
        params string[] messages)
    {
        return result.WithWarnings(
            (IEnumerable<string>)messages);
    }

    public static Result<T> WithWarnings<T>(
        this Result<T> result,
        IEnumerable<string> messages)
    {
        ArgumentNullException.ThrowIfNull(result);
        ArgumentNullException.ThrowIfNull(messages);
        foreach ( var message in messages )
        {
            result.WithSuccess(
                new Warning(message));
        }
        return result;
    }

    public static bool IsWarning(
        this IReason reason)
    {
        ArgumentNullException.ThrowIfNull(reason);
        return reason.Metadata.TryGetValue(
                IsWarningMetadataKey,
                out var value)
                && value is true;
    }

    public static List<string> GetMessages(
        this IReason reason)
    {
        ArgumentNullException.ThrowIfNull(reason);
        return reason.Metadata.TryGetValue(
                WarningMessagesMetadataKey,
                out var value)
                && value is List<string> messages
                ? [.. messages]
                : [];
    }

    public static bool HasWarnings(
        this Result result)
    {
        ArgumentNullException.ThrowIfNull(result);
        return result.Successes.Any(IsWarning);
    }

    public static bool HasWarnings<T>(
        this Result<T> result)
    {
        ArgumentNullException.ThrowIfNull(result);
        return result.Successes.Any(IsWarning);
    }

    public static List<string> GetAllWarningMessages(
        this Result result)
    {
        ArgumentNullException.ThrowIfNull(result);
        return
        [
            .. result.Successes
               .Where(IsWarning)
               .SelectMany(GetMessages)
        ];
    }

    public static List<string> GetAllWarningMessages<T>(
        this Result<T> result)
    {
        ArgumentNullException.ThrowIfNull(result);
        return
        [
            .. result.Successes
               .Where(IsWarning)
               .SelectMany(GetMessages)
        ];
    }

    #endregion

    #region EXTENSIONS    

    public static Dictionary<string, string[]>
        ToErrorDictionary(
            this IResultBase result)
    {
        ArgumentNullException.ThrowIfNull(result);
        return result.Errors
            .GroupBy(
                GetErrorKey,
                StringComparer.Ordinal)
            .ToDictionary(
                group => group.Key,
                group => group
                    .Select(error => error.Message)
                    .ToArray(),
                StringComparer.Ordinal);
    }

    public static string GetErrorMessage(
        this IResultBase result)
    {
        ArgumentNullException.ThrowIfNull(result);
        return result.IsFailed
            ? string.Join(
                ", ",
                result.Errors.Select(
                    error => error.Message))
            : string.Empty;
    }

    public static Result Merge(Result result, params Result[] results)
    {
        return Result.Merge([result, .. results]);
    }

    #endregion

    #region PRIVATE METHODS   

    private static List<IError> CreateValidationErrors(
        (string message, string? propertyName)[] errors)
    {
        ArgumentNullException.ThrowIfNull(errors);
        if ( errors.Length == 0 )
        {
            throw new ArgumentException(
                "Au moins une erreur de validation est requise.",
                nameof(errors));
        }
        return [.. errors
            .Select(
                error => (IError)new ValidationError(
                    error.message,
                    error.propertyName))];
    }

    private static List<T> MaterializeRequired<T>(
        IEnumerable<T> values,
        string parameterName,
        string emptyCollectionMessage)
    {
        ArgumentNullException.ThrowIfNull(values);
        var materializedValues = values.ToList();
        if ( materializedValues.Count == 0 )
        {
            throw new ArgumentException(
                emptyCollectionMessage,
                parameterName);
        }
        return materializedValues;
    }

    private static string GetErrorKey(
        IError error)
    {
        return error is ValidationError
        {
            PropertyName: not null
        } validationError
            ? validationError.PropertyName
            : GeneralErrorKey;
    }

    #endregion
}
