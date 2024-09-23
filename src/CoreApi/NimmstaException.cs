namespace Nimmsta.Net.CoreApi;

public class NimmstaException : Exception
{
    public NimmstaException()
    {
    }

    public NimmstaException(string? message)
        : base(message)
    {
    }

    public NimmstaException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
