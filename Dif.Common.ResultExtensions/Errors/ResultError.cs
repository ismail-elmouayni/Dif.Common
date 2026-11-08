namespace Dif.Common.ResultExtensions.Errors;

using FluentResults;

public class ResultError : Error
{
    public ResultError(string message) : base(message)
    {
    }

    public ResultError(string message, Dictionary<string, object> metadata) : base(message)
    {
        if (metadata != null)
        {
            foreach (var kvp in metadata)
            {
                WithMetadata(kvp.Key, kvp.Value);
            }
        }
    }
}
