namespace Crm.Application.Exceptions;

public class AdPlatformException : Exception
{
    public bool IsTokenExpired { get; }

    public AdPlatformException(string message, bool isTokenExpired = false)
        : base(message)
    {
        IsTokenExpired = isTokenExpired;
    }
}
