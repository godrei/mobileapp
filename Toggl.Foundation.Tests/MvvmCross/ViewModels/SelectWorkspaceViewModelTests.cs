using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Toggl.Foundation.MvvmCross.ViewModels;
using Toggl.Foundation.Tests.Generators;
using Toggl.PrimeRadiant.Models;
using Xunit;

namespace Toggl.Foundation.Tests.MvvmCross.ViewModels
{
    public sealed class SelectWorkspaceViewModelTests
    {
        public abstract class SelectWorkspaceViewModelTest : BaseViewModelTests<SelectWorkspaceViewModel>
        {
            protected override SelectWorkspaceViewModel CreateViewModel()
                => new SelectWorkspaceViewModel(DataSource, NavigationService);

            protected List<IDatabaseWorkspace> GenerateWorkspaceList() =>
                Enumerable.Range(0, 10).Select(i =>
                {
                    var workspace = Substitute.For<IDatabaseWorkspace>();
                    workspace.Id.Returns(i);
                    workspace.Name.Returns(i.ToString());
                    return workspace;
                }).ToList();
        }

        public sealed class TheConstructor : SelectWorkspaceViewModelTest
        {
            [Theory]
            [ClassData(typeof(TwoParameterConstructorTestData))]
            public void ThrowsIfAnyOfTheArgumentsIsNull(bool useDataSource, bool useNavigationService)
            {
                var dataSource = useDataSource ? DataSource : null;
                var navigationService = useNavigationService ? NavigationService : null;

                Action tryingToConstructWithEmptyParameters =
                    () => new SelectWorkspaceViewModel(dataSource, navigationService);

                tryingToConstructWithEmptyParameters
                    .ShouldThrow<ArgumentNullException>();
            }
        }

        public sealed class TheInitializeMethod : SelectWorkspaceViewModelTest
        {
            [Fact]
            public async Task AddsAllWorkspacesToTheListOfSuggestions()
            {
                var workspaces = GenerateWorkspaceList();
                DataSource.Workspaces.GetAll().Returns(Observable.Return(workspaces));

                await ViewModel.Initialize();

                ViewModel.Suggestions.Should().HaveCount(10);
            }
        }

        public sealed class TheCloseCommand : SelectWorkspaceViewModelTest
        {
            [Fact]
            public async Task ClosesTheViewModel()
            {
                await ViewModel.Initialize();

                await ViewModel.CloseCommand.ExecuteAsync();

                await NavigationService.Received()
                    .Close(Arg.Is(ViewModel), Arg.Any<long?>());
            }

            [Fact]
            public async Task ReturnsNull()
            {
                await ViewModel.Initialize();

                ViewModel.CloseCommand.ExecuteAsync().Wait();

                await NavigationService.Received()
                    .Close(Arg.Is(ViewModel), null);
            }
        }

        public sealed class TheSelectWorkspaceCommand : SelectWorkspaceViewModelTest
        {
            private readonly IDatabaseWorkspace Workspace = Substitute.For<IDatabaseWorkspace>();

            [Fact]
            public async Task ClosesTheViewModel()
            {
                ViewModel.SelectWorkspaceCommand.Execute(Workspace);

                await NavigationService.Received()
                    .Close(Arg.Is(ViewModel), Arg.Any<long?>());
            }

            [Fact]
            public async Task ReturnsTheSelectedWorkspaceId()
            {
                const long expectedId = 10;
                Workspace.Id.Returns(expectedId);

                ViewModel.SelectWorkspaceCommand.Execute(Workspace);

                await NavigationService.Received().Close(
                    Arg.Is(ViewModel),
                    Arg.Is<long?>(expectedId)
                );
            }
        }

        public sealed class TheTextProperty : SelectWorkspaceViewModelTest
        {
            [Fact]
            public async Task FiltersTheSuggestionsWhenItChanges()
            {
                var workspaces = GenerateWorkspaceList();
                DataSource.Workspaces.GetAll().Returns(Observable.Return(workspaces));
                await ViewModel.Initialize();

                ViewModel.Text = "0";

                ViewModel.Suggestions.Should().HaveCount(1);
            }
        }
    }
}
