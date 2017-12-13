using System;
using Toggl.Foundation.MvvmCross.Services;

namespace Toggl.Giskard.Services
{
    public sealed class OnePasswordService : IPasswordManagerService
    {
        public bool IsAvailable => throw new NotImplementedException();

        public IObservable<PasswordManagerResult> GetLoginInformation()
        {
            throw new NotImplementedException();
        }
    }
}
