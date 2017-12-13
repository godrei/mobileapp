using System;
using Toggl.Foundation.Login;

namespace Toggl.Giskard.Services
{
    public sealed class GoogleService : IGoogleService
    {
        public IObservable<string> GetAuthToken()
            => throw new NotImplementedException();
    }
}
