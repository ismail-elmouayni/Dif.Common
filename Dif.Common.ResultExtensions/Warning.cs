namespace Dif.Common.ResultExtensions;

using FluentResults;

public class Warning : Success
{
    public Warning(string message) : base(message)
    {
        WithMetadata("IsWarning", true);
        WithMetadata("WarningMessages", new List<string> { message });
    }

    public Warning(IEnumerable<string> messages) : base(string.Join("; ", messages))
    {
        WithMetadata("IsWarning", true);
        WithMetadata("WarningMessages", new List<string>(messages));
    }

    public Warning(string message, Dictionary<string, object> metadata) : base(message)
    {
        WithMetadata("IsWarning", true);
        WithMetadata("WarningMessages", new List<string> { message });

        if (metadata != null)
        {
            foreach (var kvp in metadata)
            {
                WithMetadata(kvp.Key, kvp.Value);
            }
        }
    }
}
