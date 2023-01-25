using WhoAskedBackend.Services.ContextServices;

namespace WhoAskedBackend.Services;

public class ModelServiceBase
{
    public void EnsureNotNull(object value, string parameterName)
    {
        if (value == null)
            throw CreateException($"Parameter {parameterName} cannot be null.");
    }

    public ServiceException CreateException(string message, Exception? innerException = null)
        => new(typeof(UserService), message, innerException);
}