using CoreGraphics;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Binding.iOS;
using MvvmCross.iOS.Views;
using MvvmCross.iOS.Views.Presenters.Attributes;
using Toggl.Daneel.Extensions;
using Toggl.Daneel.ViewSources;
using Toggl.Foundation.MvvmCross.Helper;
using Toggl.Foundation.MvvmCross.ViewModels;
using UIKit;
using static Toggl.Daneel.Extensions.LayoutConstraintExtensions;

namespace Toggl.Daneel.ViewControllers
{
    [MvxChildPresentation]
    public sealed partial class ReportsViewController : MvxViewController<ReportsViewModel>
    {
        private const int calendarHeight = 338;

        private UIButton titleButton;

        internal UIView CalendarContainerView => CalendarContainer;

        internal bool CalendarIsVisible => !CalendarContainer.Hidden;

        public ReportsViewController() : base(nameof(ReportsViewController), null)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            prepareViews();

            var source = new ReportsTableViewSource(ReportsTableView);
            ReportsTableView.Source = source;


            var bindingSet = this.CreateBindingSet<ReportsViewController, ReportsViewModel>();

            bindingSet.Bind(source).To(vm => vm.Segments);
            bindingSet.Bind(titleButton).To(vm => vm.ToggleCalendarCommand);
            bindingSet.Bind(titleButton)
                      .For(v => v.BindTitle())
                      .To(vm => vm.CurrentDateRangeString);

            bindingSet.Bind(source)
                      .For(v => v.ViewModel)
                      .To(vm => vm);

            bindingSet.Bind(ReportsTableView)
                      .For(v => v.BindTap())
                      .To(vm => vm.HideCalendarCommand);

            bindingSet.Apply();
        }

        internal void ShowCalendar()
        {
            CalendarContainer.Hidden = false;
            TopCalendarConstraint.Constant = 0;
            AnimationExtensions.Animate(
                Animation.Timings.EnterTiming,
                Animation.Curves.SharpCurve,
                () => View.LayoutSubviews());
        }

        internal void HideCalendar()
        {
            TopCalendarConstraint.Constant = -calendarHeight;
            AnimationExtensions.Animate(
                Animation.Timings.EnterTiming,
                Animation.Curves.SharpCurve,
                () => View.LayoutSubviews(),
                () => CalendarContainer.Hidden = true);
        }

        private void prepareViews()
        {
            TopConstraint.AdaptForIos10(NavigationController.NavigationBar);

            // Title view
            NavigationItem.TitleView = titleButton = new UIButton(new CGRect(0, 0, 200, 40));
            titleButton.SetTitleColor(UIColor.Black, UIControlState.Normal);

            // Calendar configuration
            CalendarHeightConstraint.Constant = 0;
            CalendarContainer.Hidden = true;
        }
    }
}

