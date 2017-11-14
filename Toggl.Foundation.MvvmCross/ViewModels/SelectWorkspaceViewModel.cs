using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using Toggl.Foundation.DataSources;
using Toggl.Multivac;
using Toggl.Multivac.Extensions;
using Toggl.PrimeRadiant.Models;

namespace Toggl.Foundation.MvvmCross.ViewModels
{
    [Preserve(AllMembers = true)]
    public sealed class SelectWorkspaceViewModel : MvxViewModelResult<long?>
    {
        private readonly ITogglDataSource dataSource;
        private readonly IMvxNavigationService navigationService;

        private IEnumerable<IDatabaseWorkspace> allWorkspaces;

        public string Text { get; set; } = "";

        public IMvxAsyncCommand CloseCommand { get; }

        public IMvxAsyncCommand<IDatabaseWorkspace> SelectWorkspaceCommand { get; }

        public MvxObservableCollection<IDatabaseWorkspace> Suggestions { get; }
            = new MvxObservableCollection<IDatabaseWorkspace>();

        public SelectWorkspaceViewModel(ITogglDataSource dataSource, IMvxNavigationService navigationService)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));

            this.dataSource = dataSource;
            this.navigationService = navigationService;

            CloseCommand = new MvxAsyncCommand(close);
            SelectWorkspaceCommand = new MvxAsyncCommand<IDatabaseWorkspace>(selectWorkspace);
        }

        public override async Task Initialize()
        {
            await base.Initialize();

            allWorkspaces = await dataSource.Workspaces.GetAll();
            Suggestions.AddRange(allWorkspaces);
        }

        private void OnTextChanged()
        {
            Suggestions.Clear();
            Suggestions.AddRange(
                allWorkspaces.Where(w => w.Name.ContainsIgnoringCase(Text))
            );
        }

        private Task close()
            => navigationService.Close(this, null);

        private Task selectWorkspace(IDatabaseWorkspace workspace)
            => navigationService.Close(this, workspace.Id);
    }
}
