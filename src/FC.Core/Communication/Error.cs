namespace FC.Core.Communication;

/// <summary>
///     Represents an error in a result, for demonstration purposes we use simple predefined errors.
/// </summary>
public sealed record Error(string Code, string Message)
{
    public static readonly Error None = new("None", "No error");
    public static readonly Error NullValue = new("NullValue", "Value is null");
    public static readonly Error Commit = new("Commit", "Erro ao persistir dados");

    public Error WithMessageParam(params object[] args)
    {
        var formattedMessage = string.Format(Message, args);
        return this with { Message = formattedMessage };
    }

    public override string ToString()
    {
        return string.IsNullOrEmpty(Code) ? $"{Code}: {Message}" : Message;
    }
}