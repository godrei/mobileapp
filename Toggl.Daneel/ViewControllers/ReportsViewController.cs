using System;
using CoreGraphics;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Binding.iOS;
using MvvmCross.iOS.Views;
using MvvmCross.iOS.Views.Presenters.Attributes;
using Toggl.Daneel.Extensions;
using Toggl.Daneel.ViewSources;
using Toggl.Foundation.MvvmCross.Converters;
using Toggl.Foundation.MvvmCross.ViewModels;
using UIKit;
using static Toggl.Daneel.Extensions.LayoutConstraintExtensions;

namespace Toggl.Daneel.ViewControllers
{
    [MvxChildPresentation]
    public sealed partial class ReportsViewController : MvxViewController<ReportsViewModel>
    {
        private UIButton titleButton;

        internal UIView CalendarContainerView => CalendarContainer;

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
            
            bindingSet.Bind(CalendarHeightConstraint)
                      .For(v => v.BindAnimatedConstant())
                      .To(vm => vm.IsCalendarVisible)
                      .WithConversion(new BoolToConstantValueConverter<nfloat>(338, 0));

            bindingSet.Bind(source)
                      .For(v => v.ViewModel)
                      .To(vm => vm);

            bindingSet.Bind(ReportsTableView)
                      .For(v => v.BindTap())
                      .To(vm => vm.HideCalendarCommand);

            bindingSet.Apply();
        }

        private void prepareViews()
        {
            TopConstraint.AdaptForIos10(NavigationController.NavigationBar);

            NavigationItem.TitleView = titleButton = new UIButton(new CGRect(0, 0, 200, 40));
            titleButton.SetTitleColor(UIColor.Black, UIControlState.Normal);
        }
    }
}

