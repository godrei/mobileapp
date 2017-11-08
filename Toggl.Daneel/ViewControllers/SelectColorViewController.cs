using CoreGraphics;
using MvvmCross.Binding.BindingContext;
using MvvmCross.iOS.Views;
using Toggl.Daneel.Presentation.Attributes;
using Toggl.Daneel.ViewSources;
using Toggl.Foundation.MvvmCross.ViewModels;
using UIKit;

namespace Toggl.Daneel.ViewControllers
{
    [ModalDialogPresentation]
    public sealed partial class SelectColorViewController : MvxViewController<SelectColorViewModel>
    {
        public SelectColorViewController()
            : base(nameof(SelectColorViewController), null)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var source = new ColorSelectionCollectionViewSource(ColorCollectionView);
            prepareViews(source);

            var bindingSet = this.CreateBindingSet<SelectColorViewController, SelectColorViewModel>();

            //Collection View
            bindingSet.Bind(source).To(vm => vm.SelectableColors);
            bindingSet.Bind(source)
                      .For(v => v.SelectionChangedCommand)
                      .To(vm => vm.SelectColorCommand);

            //Commands
            bindingSet.Bind(SaveButton).To(vm => vm.SaveCommand);
            bindingSet.Bind(CloseButton).To(vm => vm.CloseCommand);

            bindingSet.Apply();
        }

        private void prepareViews(ColorSelectionCollectionViewSource source)
        {
            var screenWidth = UIScreen.MainScreen.Bounds.Width;
            PreferredContentSize = new CGSize
            {
                //ScreenWidth - 32 for 16pt margins on both sides
                Width = screenWidth > 320 ? screenWidth - 32 : 312,
                Height = View.Frame.Height
            };

            ColorCollectionView.Source = source;
        }
    }
}

