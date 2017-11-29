using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using PropertyChanged;
using Toggl.Foundation.DataSources;
using Toggl.Foundation.DTOs;
using Toggl.Foundation.MvvmCross.Collections;
using Toggl.Multivac;
using Toggl.Multivac.Extensions;
using Toggl.PrimeRadiant.Models;

namespace Toggl.Foundation.MvvmCross.ViewModels
{
    [Preserve(AllMembers = true)]
    public sealed class TimeEntriesLogViewModel : MvxViewModel
    {
        private readonly CompositeDisposable disposeBag = new CompositeDisposable();

        private readonly ITimeService timeService;
        private readonly ITogglDataSource dataSource;
        private readonly IMvxNavigationService navigationService;

        public bool IsWelcome { get; set; }

        public MvxObservableCollection<TimeEntryViewModelCollection> TimeEntries { get; }
            = new MvxObservableCollection<TimeEntryViewModelCollection>();

        [DependsOn(nameof(TimeEntries))]
        public bool IsEmpty => !TimeEntries.Any();

        [DependsOn(nameof(IsWelcome))]
        public string EmptyStateTitle
            => IsWelcome
            ? Resources.TimeEntriesLogEmptyStateWelcomeTitle
            : Resources.TimeEntriesLogEmptyStateTitle;

        [DependsOn(nameof(IsWelcome))]
        public string EmptyStateText
            => IsWelcome
            ? Resources.TimeEntriesLogEmptyStateWelcomeText
            : Resources.TimeEntriesLogEmptyStateText;

        public IMvxAsyncCommand<TimeEntryViewModel> EditCommand { get; }

        public IMvxAsyncCommand<TimeEntryViewModel> ContinueTimeEntryCommand { get; }

        public TimeEntriesLogViewModel(ITogglDataSource dataSource, ITimeService timeService,
            IMvxNavigationService navigationService)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));

            this.dataSource = dataSource;
            this.timeService = timeService;
            this.navigationService = navigationService;

            EditCommand = new MvxAsyncCommand<TimeEntryViewModel>(edit);
            ContinueTimeEntryCommand = new MvxAsyncCommand<TimeEntryViewModel>(continueTimeEntry);
        }

        public override async Task Initialize()
        {
            await base.Initialize();

            await reset();

            var deleteDisposable = 
                dataSource.TimeEntries.TimeEntryDeleted
                          .Subscribe(safeRemoveTimeEntry);

            var updateDisposable =
                dataSource.TimeEntries.TimeEntryUpdated
                          .Subscribe(onTimeEntryUpdated);

            var createDisposable =
                dataSource.TimeEntries.TimeEntryCreated
                    .Where(isNotRunning)
                    .Subscribe(safeInsertTimeEntry);

            var midnightDisposable =
                timeService.MidnightObservable
                    .Subscribe(onMidnight);

            disposeBag.Add(createDisposable);
            disposeBag.Add(updateDisposable);
            disposeBag.Add(deleteDisposable);
            disposeBag.Add(midnightDisposable);
        }
        
        private async Task reset()
        {
            TimeEntries.Clear();

            var timeEntries = await dataSource.TimeEntries.GetAll();

            timeEntries
                .Where(isNotRunning)
                .OrderByDescending(te => te.Start)
                .Select(te => new TimeEntryViewModel(te))
                .GroupBy(te => te.Start.LocalDateTime.Date)
                .Select(grouping => new TimeEntryViewModelCollection(grouping.Key, grouping))
                .ForEach(addTimeEntries);
        }

        private void addTimeEntries(TimeEntryViewModelCollection collection)
        {
            TimeEntries.Add(collection);
            
            RaisePropertyChanged(nameof(IsEmpty));
        }

        private void onTimeEntryUpdated((long Id, IDatabaseTimeEntry Entity) tuple)
        {
            var timeEntry = tuple.Entity;
            safeRemoveTimeEntry(tuple.Id);

            if (timeEntry == null || timeEntry.IsRunning() || timeEntry.IsDeleted) return;
            safeInsertTimeEntry(timeEntry);
        }

        private void safeInsertTimeEntry(IDatabaseTimeEntry timeEntry)
        {
            var indexDate = timeEntry.Start.LocalDateTime.Date;
            var timeEntriesInDay = new List<TimeEntryViewModel> { new TimeEntryViewModel(timeEntry) };

            var collection = TimeEntries.FirstOrDefault(x => x.Date == indexDate);
            if (collection != null)
            {
                timeEntriesInDay.AddRange(collection);
                TimeEntries.Remove(collection);
            }

            var newCollection = new TimeEntryViewModelCollection(indexDate, timeEntriesInDay.OrderByDescending(te => te.Start));

            var foundIndex = TimeEntries.IndexOf(TimeEntries.FirstOrDefault(x => x.Date < indexDate));
            var indexToInsert = foundIndex == -1 ? TimeEntries.Count : foundIndex;
            TimeEntries.Insert(indexToInsert, newCollection);

            RaisePropertyChanged(nameof(IsEmpty));
        }

        private void safeRemoveTimeEntry(long id)
        {
            var collection = TimeEntries.FirstOrDefault(c => c.Any(vm => vm.Id == id));
            if (collection == null) return;

            var viewModel = collection.First(vm => vm.Id == id);

            var indexToInsert = TimeEntries.IndexOf(collection);
            collection.Remove(viewModel);
            TimeEntries.Remove(collection);

            if (collection.Any())
            {
                var newCollection = new TimeEntryViewModelCollection(collection.Date.LocalDateTime, collection);
                TimeEntries.Insert(indexToInsert, newCollection);
            }

            RaisePropertyChanged(nameof(IsEmpty));
        }

        private async void onMidnight(DateTimeOffset midnight)
            => await reset();

        private bool isNotRunning(IDatabaseTimeEntry timeEntry) => !timeEntry.IsRunning();

        private Task edit(TimeEntryViewModel timeEntryViewModel)
            => navigationService.Navigate<EditTimeEntryViewModel, long>(timeEntryViewModel.Id);

        private async Task continueTimeEntry(TimeEntryViewModel timeEntryViewModel)
        {
            await dataSource.User
                .Current()
                .Select(user => new StartTimeEntryDTO
                {
                    UserId = user.Id,
                    TaskId = timeEntryViewModel.TaskId,
                    WorkspaceId = user.DefaultWorkspaceId,
                    Billable = timeEntryViewModel.Billable,
                    StartTime = timeService.CurrentDateTime, 
                    ProjectId = timeEntryViewModel.ProjectId,
                    Description = timeEntryViewModel.Description
                })
                .SelectMany(dataSource.TimeEntries.Start)
                .Do(_ => dataSource.SyncManager.PushSync());
        }
    }
}
