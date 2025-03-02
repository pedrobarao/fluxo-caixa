namespace FC.Core.Messages;

public abstract record Message
{
    protected Message()
    {
        MessageType = GetType().Name;
    }

    public string MessageType { get; protected set; }
}