namespace Dif.Common.ResultExtensions;

using FluentResults;

public class ResultReason : IReason
{
    private readonly string _message;
    private readonly Dictionary<string, object> _metaData = new Dictionary<string, object>();

    public string Message => _message;

    public ResultReason(string message, Dictionary<string, object> metadata = null)
    {
        _message = message;

        if (metadata != null)
        {
            _metaData = metadata;
        }
    }

    public Dictionary<string, object> Metadata => _metaData;
}
