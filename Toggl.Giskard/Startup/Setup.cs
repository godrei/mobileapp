using System;
using System.Reactive.Concurrency;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using MvvmCross.Droid.Platform;
using MvvmCross.Platform;
using MvvmCross.Platform.Platform;
using MvvmCross.Platform.Plugins;
using Android.Content;
using Toggl.Foundation;
using Toggl.Foundation.DataSources;
using Toggl.Foundation.Login;
using Toggl.Foundation.MvvmCross;
using Toggl.Foundation.MvvmCross.Services;
using Toggl.Foundation.Services;
using Toggl.Foundation.Suggestions;
using Toggl.Foundation.Sync;
using Toggl.Giskard.Services;
using Toggl.PrimeRadiant;
using Toggl.PrimeRadiant.Realm;
using Toggl.Ultrawave;
using Toggl.Ultrawave.Network;
using Toggl.PrimeRadiant.Settings;

namespace Toggl.Giskard
{
    public sealed class Setup : MvxAndroidSetup
    {
        private IMvxNavigationService navigationService;

#if USE_PRODUCTION_API
        private const ApiEnvironment environment = ApiEnvironment.Production;
#else
        private const ApiEnvironment environment = ApiEnvironment.Staging;
#endif

        public Setup(Context applicationContext) 
            : base(applicationContext)
        {
        }

        protected override void InitializeLastChance()
        {
            base.InitializeLastChance();

            Mvx.RegisterSingleton<IPasswordManagerService>(new OnePasswordService());
        }

        protected override IMvxTrace CreateDebugTrace() => new DebugTrace();

        protected override IMvxApplication CreateApp() => new App();

        protected override IMvxNavigationService InitializeNavigationService(IMvxViewModelLocatorCollection collection)
            => navigationService = base.InitializeNavigationService(collection);

        protected override void InitializeApp(IMvxPluginManager pluginManager, IMvxApplication app)
        {
            base.InitializeApp(pluginManager, app);

#if !USE_PRODUCTION_API
            System.Net.ServicePointManager.ServerCertificateValidationCallback
                  += (sender, certificate, chain, sslPolicyErrors) => true;
#endif

            var scheduler = TaskPoolScheduler.Default;
            var database = new Database();
            var timeService = new TimeService(Scheduler.Default);

            var packageInfo = ApplicationContext.PackageManager.GetPackageInfo(ApplicationContext.PackageName, 0);
            var version = packageInfo.VersionName;
            var userAgent = new UserAgent("Giskard", version);

            var googleService = new GoogleService();
            var apiFactory = new ApiFactory(environment, userAgent);

            var sharedPreferences = ApplicationContext.GetSharedPreferences("giskard", FileCreationMode.WorldWriteable);
            var sharedPreferencesStorage = new SharedPreferencesAccessRestrictionStorage(sharedPreferences, timeService, Version.Parse(version));
            var apiErrorHandlingService = new ApiErrorHandlingService(navigationService, sharedPreferencesStorage);
            var retryDelayLimit = TimeSpan.FromSeconds(60);

            Func<ITogglDataSource, ISyncManager> createSyncManager(ITogglApi api)
                => dataSource => TogglSyncManager.CreateSyncManager(database, api, dataSource, timeService, retryDelayLimit, scheduler);

            ITogglDataSource createDataSource(ITogglApi api)
                => new TogglDataSource(database, timeService, apiErrorHandlingService, createSyncManager(api));

            var loginManager = new LoginManager(apiFactory, database, googleService, sharedPreferencesStorage, createDataSource);

            Mvx.RegisterSingleton<ITimeService>(timeService);
            Mvx.RegisterSingleton<IDialogService>(new DialogService());
            Mvx.RegisterSingleton<IOnboardingStorage>(sharedPreferencesStorage);
            Mvx.RegisterSingleton<IAccessRestrictionStorage>(sharedPreferencesStorage);
            Mvx.RegisterSingleton<IBrowserService>(new BrowserService());
            Mvx.RegisterSingleton<ISuggestionProviderContainer>(
                new SuggestionProviderContainer(
                    new MostUsedTimeEntrySuggestionProvider(database, timeService)
                )
            );
            Mvx.RegisterSingleton<IApiErrorHandlingService>(apiErrorHandlingService);

            var togglApp = app as App;
            togglApp.Initialize(loginManager, navigationService, sharedPreferencesStorage);
        }
    }
}
