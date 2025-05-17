namespace IdentityServer.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException() : base() { }

    public NotFoundException(string message) : base(message) { }
}

public class ConflictException : Exception
{
    public ConflictException() : base() { }

    public ConflictException(string message) : base(message) { }

    public ConflictException(string message, Exception innerException) : base(message, innerException) { }
}
