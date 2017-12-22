using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Multivac.Models;
using Toggl.Ultrawave.Network;
using Toggl.Ultrawave.Tests.Integration.Helper;

namespace Toggl.Ultrawave.Tests.Integration.BaseTests
{
    public abstract class EndpointTestBase
    {
        protected async Task<(ITogglApi togglClient, IUser user)> SetupTestUser()
        {
            var credentials = await User.Create();
            var togglApi = TogglApiWith(credentials);
            var user = await togglApi.User.Get();

            return (togglApi, user);
        }

        protected ITogglApi TogglApiWith(Credentials credentials)
            => new TogglApi(configurationFor(credentials));

        private ApiConfiguration configurationFor(Credentials credentials)
            => new ApiConfiguration(ApiEnvironment.Staging, credentials, Configuration.UserAgent);
    }
}
