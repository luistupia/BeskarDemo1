using System.Globalization;

namespace Common.Exceptions;

public class CustomException : Exception
{
    public CustomException()
    {
    }

    public CustomException(string? message) : base(message)
    {
    }

    public CustomException(string? message, params object[] args) : base(string.Format(CultureInfo.CurrentCulture,
        message ?? string.Empty, args))
    {
    }
}