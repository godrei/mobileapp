using System;
using System.Collections.Generic;
using CoreGraphics;
using Foundation;
using MvvmCross.Core.ViewModels;
using MvvmCross.Plugins.Color.iOS;
using Toggl.Daneel.Views;
using Toggl.Foundation.MvvmCross.Collections;
using Toggl.Foundation.MvvmCross.Helper;
using Toggl.Foundation.MvvmCross.ViewModels;
using UIKit;

namespace Toggl.Daneel.ViewSources
{
    public sealed class TimeEntriesLogViewSource : GroupedCollectionTableViewSource<TimeEntryViewModel>
    {
        private const int bottomPadding = 64;

        private const string cellIdentifier = nameof(TimeEntriesLogViewCell);
        private const string headerCellIdentifier = nameof(TimeEntriesLogHeaderViewCell);
        
        private IEnumerable<TimeEntryViewModelCollection> groupedItems
            => ItemsSource as IEnumerable<TimeEntryViewModelCollection>;

        public IMvxAsyncCommand<TimeEntryViewModel> ContinueTimeEntryCommand { get; set; }

        public TimeEntriesLogViewSource(UITableView tableView)
            : base(tableView, cellIdentifier, headerCellIdentifier)
        {
            tableView.TableFooterView = new UIView(new CGRect(0, 0, UIScreen.MainScreen.Bounds.Width, bottomPadding));
            tableView.TableFooterView.BackgroundColor = Color.TimeEntriesLog.SectionFooter.ToNativeColor();
            tableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            tableView.RegisterNibForCellReuse(TimeEntriesLogViewCell.Nib, cellIdentifier);
            tableView.RegisterNibForHeaderFooterViewReuse(TimeEntriesLogHeaderViewCell.Nib, headerCellIdentifier);
        }

        public override UIView GetViewForFooter(UITableView tableView, nint section)
        {
            var rect = new CGRect(0, 0, UIScreen.MainScreen.Bounds.Width, 24);
            var footerView = new UIView(rect)
            {
                BackgroundColor = Color.TimeEntriesLog.SectionFooter.ToNativeColor()
            };

            return footerView;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = base.GetCell(tableView, indexPath);

            if (cell is TimeEntriesLogViewCell logCell)
                logCell.ContinueTimeEntryCommand = ContinueTimeEntryCommand;

            return cell;
        }

        public override nfloat GetHeightForHeader(UITableView tableView, nint section) => 43;

        public override nfloat GetHeightForFooter(UITableView tableView, nint section) => 24;

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath) => 64;
    }
}
