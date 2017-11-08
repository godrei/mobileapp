using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using MvvmCross.Platform.UI;
using NSubstitute;
using Toggl.Foundation.MvvmCross.Helper;
using Toggl.Foundation.MvvmCross.ViewModels;
using Xunit;

namespace Toggl.Foundation.Tests.MvvmCross.ViewModels
{
    public sealed class SelectColorViewModelTests
    {
        public abstract class SelectColorViewModelTest : BaseViewModelTests<SelectColorViewModel>
        {
            protected override SelectColorViewModel CreateViewModel()
                => new SelectColorViewModel(NavigationService);
        }

        public sealed class TheConstructor
        {
            [Fact]
            public void ThrowsIfTheArgumentIsNull()
            {
                Action tryingToConstructWithEmptyParameter =
                    () => new SelectColorViewModel(null);

                tryingToConstructWithEmptyParameter.ShouldThrow<ArgumentNullException>();
            }
        }

        public sealed class TheSelectColorCommandCommand : SelectColorViewModelTest
        {
            [Fact]
            public void ChangesTheSelectedColor()
            {
                var initiallySelectedColor = Color.DefaultProjectColors.First();
                ViewModel.Prepare(initiallySelectedColor);
                var colorToSelect = ViewModel.SelectableColors.Last();

                ViewModel.SelectColorCommand.Execute(colorToSelect);

                ViewModel.SelectableColors.Single(c => c.Selected).Color.ARGB.Should().Be(colorToSelect.Color.ARGB);
            }
        }

        public sealed class ThePrepareCommand : SelectColorViewModelTest
        {
            [Fact]
            public void AddsFourteenItemsToTheListOfSelectableColors()
            {
                ViewModel.Prepare(MvxColors.Azure);

                ViewModel.SelectableColors.Should().HaveCount(14);
            }

            [Fact]
            public void SelectsTheColorPassedAsTheParameter()
            {
                var passedColor = Color.DefaultProjectColors.Skip(3).First();

                ViewModel.Prepare(passedColor);

                ViewModel.SelectableColors.Single(c => c.Selected).Color.ARGB.Should().Be(passedColor.ARGB);
            }

            [Fact]
            public void SelectsTheFirstColorIfThePassedColorIsNotPartOfTheDefaultColors()
            {   
                var expected = Color.DefaultProjectColors.First();

                ViewModel.Prepare(MvxColors.Azure);

                ViewModel.SelectableColors.Single(c => c.Selected).Color.ARGB.Should().Be(expected.ARGB);
            }
        }

        public class TheCloseCommand : SelectColorViewModelTest
        {
            [Fact]
            public async Task ClosesTheViewModel()
            {
                await ViewModel.CloseCommand.ExecuteAsync();

                await NavigationService.Received().Close(Arg.Is(ViewModel), Arg.Any<MvxColor>());
            }

            [Fact]
            public async Task ReturnsTheDefaultParameter()
            {
                var parameter = Color.DefaultProjectColors.Last();
                ViewModel.Prepare(parameter);

                await ViewModel.CloseCommand.ExecuteAsync();

                NavigationService.Received().Close(Arg.Is(ViewModel), Arg.Is(parameter)).Wait();
            }
        }

        public sealed class TheSaveCommand : SelectColorViewModelTest
        {
            [Fact]
            public async Task ClosesTheViewModel()
            {
                await ViewModel.CloseCommand.ExecuteAsync();

                await NavigationService.Received().Close(Arg.Is(ViewModel), Arg.Any<MvxColor>());
            }

            [Fact]
            public async Task ReturnsTheSelectedColor()
            {
                ViewModel.Prepare(MvxColors.Azure);
                var expected = ViewModel.SelectableColors.First();
                ViewModel.SelectColorCommand.Execute(ViewModel.SelectableColors.First());

                await ViewModel.SaveCommand.ExecuteAsync();

                await NavigationService.Received()
                    .Close(Arg.Is(ViewModel), Arg.Is<MvxColor>(c => c.ARGB == expected.Color.ARGB));
            }
        }
    }
}
