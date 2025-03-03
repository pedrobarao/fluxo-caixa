using FC.Core.Communication;

namespace FC.Core.Specifications;

public interface ISpecification<T>
{
    string ErrorMessage { get; }
    bool IsSatisfiedBy(T entity);
    ValidationResult Validate(T entity);
    ISpecification<T> And(ISpecification<T> specification);
    ISpecification<T> Or(ISpecification<T> specification);
    ISpecification<T> Not();
}