﻿using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using MvvmCross.Core.Navigation;
using NSubstitute;
using Toggl.Foundation.DataSources;
using Toggl.Foundation.Login;
using Toggl.Foundation.MvvmCross;
using Toggl.Foundation.MvvmCross.ViewModels;
using Toggl.Foundation.Sync;
using Toggl.Foundation.Tests.Generators;
using Toggl.PrimeRadiant.Models;
using Toggl.PrimeRadiant.Settings;
using Xunit;

namespace Toggl.Foundation.Tests.MvvmCross
{
    public sealed class AppStartTests
    {
        public abstract class AppStartTest : BaseMvvmCrossTests
        {
            protected AppStart AppStart { get; }
            protected ISyncManager SyncManager { get; } = Substitute.For<ISyncManager>();
            protected ITogglDataSource DataSource { get; } = Substitute.For<ITogglDataSource>();
            protected ILoginManager LoginManager { get; } = Substitute.For<ILoginManager>();
            protected IAccessRestrictionStorage AccessRestrictionStorage { get; } =
                Substitute.For<IAccessRestrictionStorage>();

            protected AppStartTest()
            {
                AppStart = new AppStart(LoginManager, NavigationService, AccessRestrictionStorage);
                DataSource.SyncManager.Returns(SyncManager);
                LoginManager.GetDataSourceIfLoggedIn().Returns(DataSource);
            }
        }

        public sealed class TheConstructor : AppStartTest
        {
            [Theory, LogIfTooSlow]
            [ClassData(typeof(ThreeParameterConstructorTestData))]
            public void ThrowsIfAnyOfTheArgumentsIsNull(bool userLoginManager, bool userNavigationService, bool useAccessRestrictionStorage)
            {
                var loginManager = userLoginManager ? LoginManager : null;
                var navigationService = userNavigationService ? NavigationService : null;
                var accessRestrictionStorage = useAccessRestrictionStorage ? AccessRestrictionStorage : null;

                Action tryingToConstructWithEmptyParameters =
                    () => new AppStart(loginManager, navigationService, accessRestrictionStorage);

                tryingToConstructWithEmptyParameters
                    .ShouldThrow<ArgumentNullException>();
            }
        }

        public sealed class TheStartMethod : AppStartTest
        {
            [Fact, LogIfTooSlow]
            public void ShowsTheOutdatedViewIfTheCurrentVersionOfTheAppIsOutdated()
            {
                AccessRestrictionStorage.IsClientOutdated().Returns(true);

                AppStart.Start();
                Task.Delay(10).Wait();

                NavigationService.Received().Navigate<OnboardingViewModel>();
                NavigationService.Received().Navigate<OutdatedAppViewModel>();
                LoginManager.DidNotReceive().GetDataSourceIfLoggedIn();
            }

            [Fact, LogIfTooSlow]
            public void ShowsTheOutdatedViewIfTheVersionOfTheCurrentlyUsedApiIsOutdated()
            {
                AccessRestrictionStorage.IsApiOutdated().Returns(true);

                AppStart.Start();
                Task.Delay(10).Wait();

                NavigationService.Received().Navigate<OnboardingViewModel>();
                NavigationService.Received().Navigate<OutdatedAppViewModel>();
                LoginManager.DidNotReceive().GetDataSourceIfLoggedIn();
            }

            [Fact, LogIfTooSlow]
            public void ShowsTheReLoginViewIfTheUserRevokedTheApiToken()
            {
                AccessRestrictionStorage.IsUnauthorized(Arg.Any<string>()).Returns(true);

                AppStart.Start();
                Task.Delay(10).Wait();

                SyncManager.DidNotReceive().ForceFullSync();
                NavigationService.Received().Navigate<TokenResetViewModel>();
            }

            [Fact, LogIfTooSlow]
            public void ShowsTheOutdatedViewIfTheTokenWasRevokedAndTheAppIsOutdated()
            {
                AccessRestrictionStorage.IsUnauthorized(Arg.Any<string>()).Returns(true);
                AccessRestrictionStorage.IsClientOutdated().Returns(true);

                AppStart.Start();
                Task.Delay(10).Wait();

                NavigationService.Received().Navigate<OnboardingViewModel>();
                NavigationService.Received().Navigate<OutdatedAppViewModel>();
                LoginManager.DidNotReceive().GetDataSourceIfLoggedIn();
            }

            [Fact, LogIfTooSlow]
            public void ShowsTheOutdatedViewIfTheTokenWasRevokedAndTheApiIsOutdated()
            {
                AccessRestrictionStorage.IsUnauthorized(Arg.Any<string>()).Returns(true);
                AccessRestrictionStorage.IsApiOutdated().Returns(true);

                AppStart.Start();
                Task.Delay(10).Wait();

                NavigationService.Received().Navigate<OnboardingViewModel>();
                NavigationService.Received().Navigate<OutdatedAppViewModel>();
                NavigationService.DidNotReceive().Navigate<TokenResetViewModel>();
                LoginManager.DidNotReceive().GetDataSourceIfLoggedIn();
            }

            [Fact, LogIfTooSlow]
            public void DoesNotShowTheUnauthorizedAccessViewIfUsersApiTokenChanged()
            {
                var oldApiToken = Guid.NewGuid().ToString();
                var newApiToken = Guid.NewGuid().ToString();
                var user = Substitute.For<IDatabaseUser>();
                var dataSource = Substitute.For<ITogglDataSource>();
                var userSource = Substitute.For<IUserSource>();
                user.ApiToken.Returns(newApiToken);
                userSource.Current().Returns(Observable.Return(user));
                dataSource.User.Returns(userSource);
                LoginManager.GetDataSourceIfLoggedIn().Returns(dataSource);
                AccessRestrictionStorage.IsUnauthorized(Arg.Is(oldApiToken)).Returns(true);
                AccessRestrictionStorage.IsApiOutdated().Returns(false);
                AccessRestrictionStorage.IsClientOutdated().Returns(false);

                AppStart.Start();
                Task.Delay(10).Wait();

                NavigationService.Received().Navigate<MainViewModel>();
            }

            [Fact, LogIfTooSlow]
            public void ShowsTheOnboardingViewModelIfTheUserHasNotLoggedInPreviously()
            {
                ITogglDataSource dataSource = null;
                LoginManager.GetDataSourceIfLoggedIn().Returns(dataSource);

                AppStart.Start();

                NavigationService.Received().Navigate(typeof(OnboardingViewModel));
            }

            [Fact, LogIfTooSlow]
            public void ShowsTheTimeEntriesViewModelIfTheUserHasLoggedInPreviously()
            {
                var dataSource = Substitute.For<ITogglDataSource>();
                LoginManager.GetDataSourceIfLoggedIn().Returns(dataSource);

                AppStart.Start();

                NavigationService.Received().Navigate(typeof(MainViewModel));
            }
        }
    }
}
