﻿using System;
using Toggl.Foundation.Login;
using Toggl.Multivac;
using Toggl.PrimeRadiant;
using Toggl.Ultrawave;
using Toggl.Ultrawave.Network;

namespace Toggl.Foundation
{
    public sealed class Foundation
    {
        public Version Version { get; internal set; }
        public UserAgent UserAgent { get; internal set; }
        public IApiFactory ApiFactory { get; internal set; }
        public ITogglDatabase Database { get; internal set; }
        public ITimeService TimeService { get; internal set; }
        public IGoogleService GoogleService { get; internal set; }
        public ApiEnvironment ApiEnvironment { get; internal set; }

        public static Foundation Create(
            string clientName,
            string version,
            ITogglDatabase database,
            ITimeService timeService,
            IGoogleService googleService,
            ApiEnvironment apiEnvironment)
        {
            Ensure.Argument.IsNotNull(version, nameof(version));
            Ensure.Argument.IsNotNull(database, nameof(database));
            Ensure.Argument.IsNotNull(clientName, nameof(clientName));
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));
            Ensure.Argument.IsNotNull(googleService, nameof(googleService));

            var userAgent = new UserAgent(clientName, version);

            var foundation = new Foundation
            {
                Database = database,
                UserAgent = userAgent,
                TimeService = timeService,
                GoogleService = googleService,
                ApiEnvironment = apiEnvironment,
                Version = Version.Parse(version),
                ApiFactory = new ApiFactory(apiEnvironment, userAgent)
            };

            return foundation;
        }

        internal Foundation() { }
    }
}
