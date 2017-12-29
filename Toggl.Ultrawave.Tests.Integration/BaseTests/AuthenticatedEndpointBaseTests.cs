﻿using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Toggl.Multivac;
using Toggl.Multivac.Extensions;
using Toggl.Ultrawave.Exceptions;
using Toggl.Ultrawave.Network;
using Xunit;

namespace Toggl.Ultrawave.Tests.Integration.BaseTests
{
    public abstract class AuthenticatedEndpointBaseTests<T> : EndpointTestBase
    {
        protected ITogglApi ValidApi { get; private set; }

        protected abstract IObservable<T> CallEndpointWith(ITogglApi togglApi);

        protected Func<Task> CallingEndpointWith(ITogglApi togglApi)
            => async () => await CallEndpointWith(togglApi);

        [Fact, LogTestInfo]
        public async Task WorksWithPassword()
        {
            var credentials = await User.Create();
            ValidApi = TogglApiWith(credentials);

            CallingEndpointWith(ValidApi).ShouldNotThrow();
        }

        [Fact, LogTestInfo]
        public async Task WorksWithApiToken()
        {
            var (_, user) = await SetupTestUser();
            var apiTokenCredentials = Credentials.WithApiToken(user.ApiToken);
            ValidApi = TogglApiWith(apiTokenCredentials);

            CallingEndpointWith(ValidApi).ShouldNotThrow();
        }

        [Fact, LogTestInfo]
        public async Task FailsForNonExistingUser()
        {
            var (validApi, _) = await SetupTestUser();
            ValidApi = validApi;
            var email = $"non-existing-email-{Guid.NewGuid()}@ironicmocks.toggl.com".ToEmail();
            var wrongCredentials = Credentials
                .WithPassword(email, "123456789".ToPassword());

            CallingEndpointWith(TogglApiWith(wrongCredentials)).ShouldThrow<UnauthorizedException>();
        }

        [Fact, LogTestInfo]
        public async Task FailsWithWrongPassword()
        {
            var (email, password) = await User.CreateEmailPassword();
            var correctCredentials = Credentials.WithPassword(email, password);
            var incorrectPassword = Password.FromString($"{password}111");
            var wrongCredentials = Credentials.WithPassword(email, incorrectPassword);
            ValidApi = TogglApiWith(correctCredentials);

            CallingEndpointWith(TogglApiWith(wrongCredentials)).ShouldThrow<UnauthorizedException>();
        }

        [Fact, LogTestInfo]
        public async Task FailsWithWrongApiToken()
        {
            var (validApi, _) = await SetupTestUser();
            ValidApi = validApi;
            var wrongApiToken = Guid.NewGuid().ToString("N");
            var wrongApiTokenCredentials = Credentials.WithApiToken(wrongApiToken);

            CallingEndpointWith(TogglApiWith(wrongApiTokenCredentials)).ShouldThrow<UnauthorizedException>();
        }

        [Fact, LogTestInfo]
        public async Task FailsWithoutCredentials()
        {
            var (validApi, _) = await SetupTestUser();
            ValidApi = validApi;

            CallingEndpointWith(TogglApiWith(Credentials.None)).ShouldThrow<UnauthorizedException>();
        }
    }
}
