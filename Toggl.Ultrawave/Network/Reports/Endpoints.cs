using System;
using Toggl.Ultrawave.Helpers;

namespace Toggl.Ultrawave.Network.Reports
{
    internal sealed class Endpoints
    {
        private readonly Uri baseUrl;

        public ProjectEndpoints Projects => new ProjectEndpoints(baseUrl);

        public Endpoints(ApiEnvironment environment)
        {
            baseUrl = BaseUrls.ForReports(environment);
        }
    }
}
