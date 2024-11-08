namespace TestGenerator.Core.Exceptions;

public class UnauthorizedException : HttpServiceException
{
    public UnauthorizedException() : base()
    {
    }

    public UnauthorizedException(string message) : base(message)
    {
    }

    public UnauthorizedException(string message, Exception inner) : base(message, inner)
    {
    }
}