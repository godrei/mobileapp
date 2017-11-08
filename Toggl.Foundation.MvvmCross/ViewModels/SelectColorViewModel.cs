using System.Linq;
using System.Threading.Tasks;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform.UI;
using Toggl.Foundation.MvvmCross.Helper;
using Toggl.Multivac;

namespace Toggl.Foundation.MvvmCross.ViewModels
{
    [Preserve(AllMembers = true)]
    public class SelectColorViewModel : MvxViewModel<MvxColor, MvxColor>
    {
        private readonly IMvxNavigationService navigationService;

        private MvxColor defaultColor;

        public IMvxAsyncCommand SaveCommand { get; set; }

        public IMvxAsyncCommand CloseCommand { get; set; }

        public IMvxCommand<SelectableColorViewModel> SelectColorCommand { get; set; }

        public MvxObservableCollection<SelectableColorViewModel> SelectableColors { get; } =
            new MvxObservableCollection<SelectableColorViewModel>();
        
        public SelectColorViewModel(IMvxNavigationService navigationService)
        {
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));

            this.navigationService = navigationService;

            SaveCommand = new MvxAsyncCommand(save);
            CloseCommand = new MvxAsyncCommand(close);
            SelectColorCommand = new MvxCommand<SelectableColorViewModel>(selectColor);
        }

        public override void Prepare(MvxColor parameter)
        {
            defaultColor = parameter;

            SelectableColors.AddRange(
                Color.DefaultProjectColors.Select(color => new SelectableColorViewModel(color, color == parameter))
            );

            if (SelectableColors.All(color => !color.Selected))
                SelectableColors.First().Selected = true;
        }

        private void selectColor(SelectableColorViewModel color)
        {
            foreach (var selectableColor in SelectableColors)
                selectableColor.Selected = color.Color.ARGB == selectableColor.Color.ARGB;
        }

        private Task close()
            => navigationService.Close(this, defaultColor);

        private Task save()
            => navigationService.Close(this, SelectableColors.FirstOrDefault(x => x.Selected).Color);
    }
}
