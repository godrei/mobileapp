using System;
using System.Reactive.Linq;
using Toggl.Foundation.DataSources;
using Toggl.Foundation.Models;
using Toggl.Multivac;
using Toggl.Multivac.Models;
using Toggl.PrimeRadiant;
using Toggl.PrimeRadiant.Settings;
using Toggl.Ultrawave;
using Toggl.Ultrawave.Network;

namespace Toggl.Foundation.Login
{
    public sealed class LoginManager : ILoginManager
    {
        private readonly IApiFactory apiFactory;
        private readonly ITogglDatabase database;
        private readonly IGoogleService googleService;
        private readonly IAccessRestrictionStorage accessRestrictionStorage;
        private readonly Func<ITogglApi, ITogglDataSource> createDataSource;

        public LoginManager(
            IApiFactory apiFactory,
            ITogglDatabase database,
            IGoogleService googleService,
            IAccessRestrictionStorage accessRestrictionStorage,
            Func<ITogglApi, ITogglDataSource> createDataSource)
        {
            Ensure.Argument.IsNotNull(database, nameof(database));
            Ensure.Argument.IsNotNull(apiFactory, nameof(apiFactory));
            Ensure.Argument.IsNotNull(accessRestrictionStorage, nameof(accessRestrictionStorage));
            Ensure.Argument.IsNotNull(googleService, nameof(googleService));
            Ensure.Argument.IsNotNull(createDataSource, nameof(createDataSource));

            this.database = database;
            this.apiFactory = apiFactory;
            this.accessRestrictionStorage = accessRestrictionStorage;
            this.googleService = googleService;
            this.createDataSource = createDataSource;
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
                    .Select(dataSourceFromUser);
        }

        public IObservable<ITogglDataSource> LoginWithGoogle()
            => database
                .Clear()
                .SelectMany(_ => googleService.GetAuthToken())
                .Select(Credentials.WithGoogleToken)
                .Select(apiFactory.CreateApiWith)
                .SelectMany(api => api.User.GetWithGoogle())
                .Select(User.Clean)
                .SelectMany(database.User.Create)
                .Select(dataSourceFromUser);

        public IObservable<ITogglDataSource> SignUp(Email email, string password)
        {
            if (!email.IsValid)
                throw new ArgumentException("A valid email must be provided when trying to signup");
            Ensure.Argument.IsNotNullOrWhiteSpaceString(password, nameof(password));

            return database
                    .Clear()
                    .SelectMany(_ => apiFactory.CreateApiWith(Credentials.None).User.SignUp(email, password))
                    .Select(User.Clean)
                    .SelectMany(database.User.Create)
                    .Select(dataSourceFromUser);
        }

        public IObservable<ITogglDataSource> SignUpWithGoogle()
            => database
                .Clear()
                .SelectMany(_ => googleService.GetAuthToken())
                .SelectMany(apiFactory.CreateApiWith(Credentials.None).User.SignUpWithGoogle)
                .Select(User.Clean)
                .SelectMany(database.User.Create)
                .Select(dataSourceFromUser);

        public IObservable<string> ResetPassword(Email email)
        {
            if (!email.IsValid)
                throw new ArgumentException("A valid email must be provided when trying to reset forgotten password.");

            var api = apiFactory.CreateApiWith(Credentials.None);
            return api.User.ResetPassword(email);
        }

        public ITogglDataSource GetDataSourceIfLoggedIn()
            => database.User
                .Single()
                .Select(dataSourceFromUser)
                .Catch(Observable.Return<ITogglDataSource>(null))
                .Wait();

        public IObservable<ITogglDataSource> RefreshToken(string password)
        {
            Ensure.Argument.IsNotNullOrWhiteSpaceString(password, nameof(password));

            return database.User
                .Single()
                .Select(user => Email.FromString(user.Email))
                .Select(email => Credentials.WithPassword(email, password))
                .Select(apiFactory.CreateApiWith)
                .SelectMany(api => api.User.Get())
                .Select(User.Clean)
                .SelectMany(database.User.Update)
                .Select(dataSourceFromUser);
        }

        private ITogglDataSource dataSourceFromUser(IUser user)
        {
            var newCredentials = Credentials.WithApiToken(user.ApiToken);
            var api = apiFactory.CreateApiWith(newCredentials);
            return createDataSource(api);
        }
    }
}
