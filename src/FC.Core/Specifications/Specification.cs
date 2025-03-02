using FC.Core.Communication;

namespace FC.Core.Specifications;

public abstract class Specification<T> : ISpecification<T>
{
    protected Specification(string errorMessage)
    {
        ErrorMessage = errorMessage;
    }

    public abstract bool IsSatisfiedBy(T entity);

    public string ErrorMessage { get; protected set; }

    public virtual ValidationResult Validate(T entity)
    {
        var isValid = IsSatisfiedBy(entity);
        var validationResult = new ValidationResult();

        if (isValid) return validationResult;

        validationResult.AddErrorRange([
            new Error(typeof(T).Name, ErrorMessage)
        ]);

        return validationResult;
    }

    public ISpecification<T> And(ISpecification<T> specification)
    {
        return new AndSpecification<T>(this, specification);
    }

    public ISpecification<T> Or(ISpecification<T> specification)
    {
        return new OrSpecification<T>(this, specification);
    }

    public ISpecification<T> Not()
    {
        return new NotSpecification<T>(this);
    }
}