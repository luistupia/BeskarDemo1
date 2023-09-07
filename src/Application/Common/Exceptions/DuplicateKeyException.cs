namespace Application.Common.Exceptions;

public class DuplicateKeyException : Exception
{
    public DuplicateKeyException()
    {
    }

    public DuplicateKeyException(string message)
        : base(message)
    {
    }

    public DuplicateKeyException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public DuplicateKeyException(string name, object key)
        : base($"Entity \"{name}\" ({key}) already exists.")
    {
    }
}
