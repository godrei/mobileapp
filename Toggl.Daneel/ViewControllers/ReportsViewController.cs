using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Binding.iOS;
using MvvmCross.iOS.Views;
using MvvmCross.Plugins.Visibility;
using Toggl.Foundation.MvvmCross.ViewModels;
using UIKit;
using static Toggl.Daneel.Extensions.LayoutConstraintExtensions;

namespace Toggl.Daneel.ViewControllers
{
    public sealed partial class ReportsViewController : MvxViewController<ReportsViewModel>
    {
        public ReportsViewController() : base(nameof(ReportsViewController), null)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            prepareViews();

            var invertedVisibilityConverter = new MvxInvertedVisibilityValueConverter();

            var bindingSet = this.CreateBindingSet<ReportsViewController, ReportsViewModel>();

            //Text
            bindingSet.Bind(this)
                      .For(v => v.Title)
                      .To(vm => vm.CurrentDateRangeString);

            //Visibility
            bindingSet.Bind(EmptyStateView)
                      .For(v => v.BindVisibility())
                      .To(vm => vm.HasData)
                      .WithConversion(invertedVisibilityConverter);

            bindingSet.Apply();
        }

        private void prepareViews()
        {
            TopConstraint.AdaptForIos10(NavigationController.NavigationBar);

            prepareDurationLabel();
        }

        private void prepareDurationLabel()
        {
            var fontSize = 24;
            var totalDuration = new NSMutableAttributedString(
                "0:00",
                new UIStringAttributes
                {
                    Font = UIFont.SystemFontOfSize(fontSize, UIFontWeight.Medium)
                }
            );
            totalDuration.Append(new NSAttributedString(
                ":00",
                new UIStringAttributes
                {
                    Font = UIFont.SystemFontOfSize(fontSize, UIFontWeight.Light)
                })
            );
            TotalDurationLabel.AttributedText = totalDuration;
        }
    }
}

