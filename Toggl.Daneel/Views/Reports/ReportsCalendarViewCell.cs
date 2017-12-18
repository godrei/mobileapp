using System;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Binding.iOS.Views;
using MvvmCross.Core.ViewModels;
using MvvmCross.Plugins.Color.iOS;
using Toggl.Daneel.Combiners;
using Toggl.Foundation.MvvmCross.Converters;
using Toggl.Foundation.MvvmCross.Helper;
using Toggl.Foundation.MvvmCross.ViewModels.Calendar;
using UIKit;

namespace Toggl.Daneel.Views
{
    public sealed partial class ReportsCalendarViewCell : MvxCollectionViewCell
    {
        private const int cornerRadius = 16;

        public static readonly NSString Key = new NSString(nameof(ReportsCalendarViewCell));
        public static readonly UINib Nib;

        static ReportsCalendarViewCell()
        {
            Nib = UINib.FromName(nameof(ReportsCalendarViewCell), NSBundle.MainBundle);
        }

        public IMvxCommand<CalendarDayViewModel> CellTappedCommand { get; set; }

        protected ReportsCalendarViewCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            BackgroundView.CornerRadius = cornerRadius;

            this.DelayBind(() =>
            {
                var backgroundColorConverter
                    = new BoolToConstantValueConverter<UIColor>(
                        Color.Calendar.SelectedDayBackgoundColor.ToNativeColor(),
                        Color.Common.Transparent.ToNativeColor());

                var bindingSet = this.CreateBindingSet<ReportsCalendarViewCell, CalendarDayViewModel>();

                //Text
                bindingSet.Bind(Text).To(vm => vm.Day);

                //Color
                bindingSet.Bind(Text)
                          .For(v => v.TextColor)
                          .ByCombining(new CalendarCellTextColorValueCombiner(),
                                       v => v.IsInCurrentMonth,
                                       v => v.Selected);

                bindingSet.Bind(BackgroundView)
                          .For(v => v.BackgroundColor)
                          .To(vm => vm.Selected)
                          .WithConversion(backgroundColorConverter);

                //Rounding
                bindingSet.Bind(BackgroundView)
                          .For(v => v.RoundLeft)
                          .To(vm => vm.IsStartOfSelectedPeriod);

                bindingSet.Bind(BackgroundView)
                          .For(v => v.RoundRight)
                          .To(vm => vm.IsEndOfSelectedPeriod);
                
                bindingSet.Apply();

            });

            AddGestureRecognizer(new UITapGestureRecognizer(
                () => CellTappedCommand?.Execute((CalendarDayViewModel)DataContext)));
        }
    }
}
