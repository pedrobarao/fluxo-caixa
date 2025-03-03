namespace FC.Core.Specifications;

public class AndSpecification<T> : Specification<T>
{
    private readonly ISpecification<T> _left;
    private readonly ISpecification<T> _right;

    public AndSpecification(ISpecification<T> left, ISpecification<T> right)
        : base($"{left.ErrorMessage} e {right.ErrorMessage}")
    {
        _left = left;
        _right = right;
    }

    public override bool IsSatisfiedBy(T entity)
    {
        var leftIsSatisfied = _left.IsSatisfiedBy(entity);
        var rightIsSatisfied = _right.IsSatisfiedBy(entity);

        if (!leftIsSatisfied)
            ErrorMessage = _left.ErrorMessage;
        else if (!rightIsSatisfied)
            ErrorMessage = _right.ErrorMessage;

        return leftIsSatisfied && rightIsSatisfied;
    }
}

public class OrSpecification<T> : Specification<T>
{
    private readonly ISpecification<T> _left;
    private readonly ISpecification<T> _right;

    public OrSpecification(ISpecification<T> left, ISpecification<T> right)
        : base($"{left.ErrorMessage} ou {right.ErrorMessage}")
    {
        _left = left;
        _right = right;
    }

    public override bool IsSatisfiedBy(T entity)
    {
        var leftIsSatisfied = _left.IsSatisfiedBy(entity);
        var rightIsSatisfied = _right.IsSatisfiedBy(entity);

        if (!leftIsSatisfied && !rightIsSatisfied)
            ErrorMessage = $"{_left.ErrorMessage} ou {_right.ErrorMessage}";

        return leftIsSatisfied || rightIsSatisfied;
    }
}

public class NotSpecification<T> : Specification<T>
{
    private readonly ISpecification<T> _specification;

    public NotSpecification(ISpecification<T> specification)
        : base($"Não ({specification.ErrorMessage})")
    {
        _specification = specification;
    }

    public override bool IsSatisfiedBy(T entity)
    {
        return !_specification.IsSatisfiedBy(entity);
    }
}