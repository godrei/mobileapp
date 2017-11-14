using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Toggl.Foundation.DataSources;
using Toggl.Foundation.Models;
using Toggl.Multivac;
using Toggl.Multivac.Models;
using Toggl.PrimeRadiant;
using Toggl.Ultrawave.Network;

namespace Toggl.Foundation.Login
{
    public sealed class LoginManager : ILoginManager
    {
        private readonly IApiFactory apiFactory;
        private readonly ITogglDatabase database;
        private readonly ITimeService timeService;
        private readonly IScheduler scheduler;

        public LoginManager(IApiFactory apiFactory, ITogglDatabase database, ITimeService timeService, IScheduler scheduler)
        {
            Ensure.Argument.IsNotNull(database, nameof(database));
            Ensure.Argument.IsNotNull(apiFactory, nameof(apiFactory));
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));
            Ensure.Argument.IsNotNull(scheduler, nameof(scheduler));

            this.database = database;
            this.apiFactory = apiFactory;
            this.timeService = timeService;
            this.scheduler = scheduler;
        }

        public IObservable<ITogglDataSource> Login(Email email, string password)
        {
            if (!email.IsValid)
                throw new ArgumentException("A valid email must be provided when trying to Login");
            Ensure.Argument.IsNotNullOrWhiteSpaceString(password, nameof(password));

            var credentials = Credentials.WithPassword(email, password);

            return database
                    .Clear()
                    .SelectMany(_ => apiFactory.CreateApiWith(credentials).User.Get())
                    .Select(User.Clean)
                    .SelectMany(database.User.Create)
                    .Select(user =>
                    {
                        var newCredentials = Credentials.WithApiToken(user.ApiToken);
                        var api = apiFactory.CreateApiWith(newCredentials);
                        return new TogglDataSource(database, api, timeService, scheduler);
                    });
        }

        public IObservable<string> ResetPassword(Email email)
        {
            if (!email.IsValid)
                throw new ArgumentException("A valid email must be provided when trying to reset forgotten password.");

            var api = apiFactory.CreateApiWith(Credentials.None);
            return api.User.ResetPassword(email);
        }

        public ITogglDataSource GetDataSourceIfLoggedIn()
        {
            return database.User
                       .Single()
                       .Select(dataSourceFromUser)
                       .Catch(Observable.Return<ITogglDataSource>(null))
                       .Wait();
        }

        private ITogglDataSource dataSourceFromUser(IUser user)
        {
            var newCredentials = Credentials.WithApiToken(user.ApiToken);
            var api = apiFactory.CreateApiWith(newCredentials);
            return new TogglDataSource(database, api, timeService, scheduler);
        }
    }
}
