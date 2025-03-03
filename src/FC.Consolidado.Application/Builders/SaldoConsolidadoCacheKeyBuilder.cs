namespace FC.Consolidado.Application.Builders;

public class SaldoConsolidadoCacheKeyBuilder
{
    private const string DateFormat = "yyyy-MM-dd";
    private readonly string _instanceName;

    public SaldoConsolidadoCacheKeyBuilder(string instanceName)
    {
        _instanceName = instanceName;
    }

    public string BuildKey(DateOnly chave)
    {
        return $"{_instanceName}:{chave.ToString(DateFormat)}";
    }
}