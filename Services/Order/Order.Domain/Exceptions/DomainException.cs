namespace Order.Domain.Exceptions;

public abstract class DomainException : Exception
{
    public int StatusCode { get; set; }

    public DomainException(string message, int statusCode)
        : base(message)
    {
        StatusCode = statusCode;
    }

    public DomainException(string message, Exception innerException, int statusCode)
        : base(message, innerException)
    { 
        StatusCode = statusCode;
    }
}
