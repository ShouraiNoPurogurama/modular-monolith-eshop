namespace Shared.Exceptions;

public class InternalServerException : Exception
{
    //1. Inherit Exception base class
    //2. Create constructor and invoke base constructor
    //3. Create props if needed
    public InternalServerException(string message)
        : base(message)
    {
    }

    public InternalServerException(string message, string details)
        : base(message)
    {
        Details = details;
    }

    public string? Details { get; }
}