using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using FluentAssertions;
using FsCheck.Xunit;
using NSubstitute;
using Toggl.Foundation.MvvmCross.ViewModels;
using Toggl.Foundation.Sync;
using Toggl.Foundation.Tests.Generators;
using Toggl.PrimeRadiant.Models;
using Xunit;

namespace Toggl.Foundation.Tests.MvvmCross.ViewModels
{
    public sealed class MainViewModelTests
    {
        public abstract class MainViewModelTest : BaseViewModelTests<MainViewModel>
        {
            protected ISubject<SyncProgress> ProgressSubject { get; } = new Subject<SyncProgress>();

            protected override MainViewModel CreateViewModel()
                => new MainViewModel(DataSource, TimeService, OnboardingStorage, NavigationService);

            protected override void AdditionalSetup()
            {
                base.AdditionalSetup();

                var syncManager = Substitute.For<ISyncManager>();
                syncManager.ProgressObservable.Returns(ProgressSubject.AsObservable());
                DataSource.SyncManager.Returns(syncManager);
            }
        }

        public sealed class TheConstructor : MainViewModelTest
        {
            [Theory, LogIfTooSlow]
            [ClassData(typeof(FourParameterConstructorTestData))]
            public void ThrowsIfAnyOfTheArgumentsIsNull(bool useDataSource, 
                                                        bool useTimeService, 
                                                        bool useOnboardingStorage,
                                                        bool useNavigationService)
            {
                var dataSource = useDataSource ? DataSource : null;
                var timeService = useTimeService ? TimeService : null;
                var navigationService = useNavigationService ? NavigationService : null;
                var onboardingStorage = useOnboardingStorage ? OnboardingStorage : null;

                Action tryingToConstructWithEmptyParameters =
                    () => new MainViewModel(dataSource, timeService, onboardingStorage, navigationService);

                tryingToConstructWithEmptyParameters
                    .ShouldThrow<ArgumentNullException>();
            }
        }

        public sealed class TheViewAppearedMethod : MainViewModelTest
        {
            [Fact, LogIfTooSlow]
            public void RequestsTheSuggestionsViewModel()
            {
                ViewModel.ViewAppeared();

                NavigationService.Received().Navigate<SuggestionsViewModel>();
            }

            [Fact, LogIfTooSlow]
            public void RequestsTheLogTimeEntriesViewModel()
            {
                ViewModel.ViewAppeared();

                NavigationService.Received().Navigate<TimeEntriesLogViewModel>();
            }
        }

        public sealed class TheStartTimeEntryCommand : MainViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async Task NavigatesToTheStartTimeEntryViewModel()
            {
                await ViewModel.StartTimeEntryCommand.ExecuteAsync();

                await NavigationService.Received()
                    .Navigate<StartTimeEntryViewModel, DateTimeOffset>(Arg.Any<DateTimeOffset>());
            }

            [Property]
            public void PassesTheCurrentDateToTheStartTimeEntryViewModel(DateTimeOffset date)
            {
                TimeService.CurrentDateTime.Returns(date);

                ViewModel.StartTimeEntryCommand.ExecuteAsync().Wait();

                NavigationService.Received().Navigate<StartTimeEntryViewModel, DateTimeOffset>(
                    Arg.Is<DateTimeOffset>(parameter => parameter == date)
                ).Wait();
            }
        }

        public sealed class TheOpenSettingsCommand : MainViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async Task NavigatesToTheSettingsViewModel()
            {
                await ViewModel.OpenSettingsCommand.ExecuteAsync();

                await NavigationService.Received().Navigate<SettingsViewModel>();
            }
        }

        public sealed class TheOpenReportsCommand : MainViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async Task NavigatesToTheReportsViewModel()
            {
                const long workspaceId = 10;
                var user = Substitute.For<IDatabaseUser>();
                user.DefaultWorkspaceId.Returns(workspaceId);
                DataSource.User.Current.Returns(Observable.Return(user));

                await ViewModel.OpenReportsCommand.ExecuteAsync();

                await NavigationService.Received().Navigate<ReportsViewModel, long>(workspaceId);
            }
        }

        public sealed class TheStopTimeEntryCommand : MainViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async Task CallsTheStopMethodOnTheDataSource()
            {
                var date = DateTimeOffset.UtcNow;
                TimeService.CurrentDateTime.Returns(date);

                await ViewModel.StopTimeEntryCommand.ExecuteAsync();

                await DataSource.TimeEntries.Received().Stop(date);
            }

            [Fact, LogIfTooSlow]
            public async Task SetsTheElapsedTimeToZero()
            {
                await ViewModel.StopTimeEntryCommand.ExecuteAsync();

                ViewModel.CurrentTimeEntryElapsedTime.Should().Be(TimeSpan.Zero);
            }

            [Fact, LogIfTooSlow]
            public async Task InitiatesPushSync()
            {
                ViewModel.StopTimeEntryCommand.Execute();

                await DataSource.SyncManager.Received().PushSync();
            }

            [Fact, LogIfTooSlow]
            public async Task DoesNotInitiatePushSyncWhenSavingFails()
            {
                DataSource.TimeEntries.Stop(Arg.Any<DateTimeOffset>())
                    .Returns(Observable.Throw<IDatabaseTimeEntry>(new Exception()));

                ViewModel.StopTimeEntryCommand.Execute();

                await DataSource.SyncManager.DidNotReceive().PushSync();
            }
        }

        public abstract class CurrentTimeEntrypropertyTest<T> : MainViewModelTest
        {
            private readonly BehaviorSubject<IDatabaseTimeEntry> currentTimeEntrySubject
                = new BehaviorSubject<IDatabaseTimeEntry>(null);

            protected abstract T ActualValue { get; }
            protected abstract T ExpectedValue { get; }
            protected abstract T ExpectedEmptyValue { get; }

            protected long TimeEntryId = 13;
            protected string Description = "Something";
            protected string Project = "Some project";
            protected string Task = "Some task";
            protected string Client = "Some client";
            protected string ProjectColor = "0000AF";

            private async Task prepare()
            {
                var timeEntry = Substitute.For<IDatabaseTimeEntry>();
                timeEntry.Id.Returns(TimeEntryId);
                timeEntry.Description.Returns(Description);
                timeEntry.Project.Name.Returns(Project);
                timeEntry.Project.Color.Returns(ProjectColor);
                timeEntry.Task.Name.Returns(Task);
                timeEntry.Project.Client.Name.Returns(Client);

                DataSource.TimeEntries.CurrentlyRunningTimeEntry.Returns(currentTimeEntrySubject.AsObservable());

                await ViewModel.Initialize();
                currentTimeEntrySubject.OnNext(timeEntry);
            }

            [Fact, LogIfTooSlow]
            public async Task IsSet()
            {
                await prepare();

                ActualValue.Should().Be(ExpectedValue);
            }

            [Fact, LogIfTooSlow]
            public async Task IsUnset()
            {
                await prepare();
                currentTimeEntrySubject.OnNext(null);

                ActualValue.Should().Be(ExpectedEmptyValue);
            }
        }

        public sealed class TheCurrentTimeEntryIdProperty : CurrentTimeEntrypropertyTest<long?>
        {
            protected override long? ActualValue => ViewModel.CurrentTimeEntryId;

            protected override long? ExpectedValue => TimeEntryId;

            protected override long? ExpectedEmptyValue => null;
        }

        public sealed class TheCurrentTimeEntryDescriptionProperty : CurrentTimeEntrypropertyTest<string>
        {
            protected override string ActualValue => ViewModel.CurrentTimeEntryDescription;

            protected override string ExpectedValue => Description;

            protected override string ExpectedEmptyValue => "";
        }

        public sealed class TheCurrentTimeEntryProjectProperty : CurrentTimeEntrypropertyTest<string>
        {
            protected override string ActualValue => ViewModel.CurrentTimeEntryProject;

            protected override string ExpectedValue => Project;

            protected override string ExpectedEmptyValue => "";
        }

        public sealed class TheCurrentTimeEntryProjectColorProperty : CurrentTimeEntrypropertyTest<string>
        {
            protected override string ActualValue => ViewModel.CurrentTimeEntryProjectColor;

            protected override string ExpectedValue => ProjectColor;

            protected override string ExpectedEmptyValue => "";
        }

        public sealed class TheCurrentTimeEntryTaskProperty : CurrentTimeEntrypropertyTest<string>
        {
            protected override string ActualValue => ViewModel.CurrentTimeEntryTask;

            protected override string ExpectedValue => Task;

            protected override string ExpectedEmptyValue => "";
        }

        public sealed class TheCurrentTimeEntryClientProperty : CurrentTimeEntrypropertyTest<string>
        {
            protected override string ActualValue => ViewModel.CurrentTimeEntryClient;

            protected override string ExpectedValue => Client;

            protected override string ExpectedEmptyValue => "";
        }
    }
}
