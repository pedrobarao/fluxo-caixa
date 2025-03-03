namespace FC.Core.Data;

public interface IUnitOfWork
{
    Task<bool> Commit();
}