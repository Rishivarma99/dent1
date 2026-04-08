namespace Dent1.Common.Exceptions;

public class GenericException : Exception
{
    public GenericException(string message = "Something went wrong") : base(message)
    {
    }
}
