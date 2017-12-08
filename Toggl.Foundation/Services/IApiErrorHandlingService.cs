using System;

namespace Toggl.Foundation.Services
{
    public interface IApiErrorHandlingService
    {
        bool TryHandleDeprecationError(Exception error);
        bool TryHandleUnauthorizedError(Exception error);
    }
}
