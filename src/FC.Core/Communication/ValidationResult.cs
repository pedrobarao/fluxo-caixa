namespace FC.Core.Communication;

public class ValidationResult
{
    public List<Error> Errors { get; } = new();
    public bool IsValid => !Errors.Any();
    public bool IsInvalid => !IsValid;

    public void AddError(Error error)
    {
        Errors.Add(error);
    }

    public void AddErrorRange(List<Error> errors)
    {
        Errors.AddRange(errors);
    }
}