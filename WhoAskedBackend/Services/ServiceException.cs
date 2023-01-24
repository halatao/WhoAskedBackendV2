namespace WorkIT_Backend.Services;

public class ServiceException : Exception
{
    public Type Service { get; private set; }

    public ServiceException(Type service, string message, Exception? cause = null)
        : base(message, cause)
    {
        this.Service = service ?? throw new ArgumentNullException();
    }
}