﻿using System;
using CoreGraphics;
using Foundation;
using MvvmCross.Binding.iOS.Views;
using Toggl.Daneel.Views.Reports;
using Toggl.Foundation.MvvmCross.ViewModels;
using UIKit;

namespace Toggl.Daneel.ViewSources
{
    public sealed class ReportsTableViewSource : MvxTableViewSource
    {
        private const int headerHeight = 535;
        private const string cellIdentifier = nameof(ReportsLegendViewCell);
        private const string headerCellIdentifier = nameof(ReportsHeaderView);

        public ReportsViewModel ViewModel { get; set; }

        public ReportsTableViewSource(UITableView tableView)
            : base(tableView)
        {
            tableView.TableHeaderView = new UIView(new CGRect(0, 0, tableView.Bounds.Size.Width, headerHeight));
            tableView.ContentInset = new UIEdgeInsets(-headerHeight, 0, 0, 0);
            tableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            tableView.RegisterNibForCellReuse(ReportsLegendViewCell.Nib, cellIdentifier);
            tableView.RegisterNibForHeaderFooterViewReuse(ReportsHeaderView.Nib, headerCellIdentifier);
        }

        public override UIView GetViewForHeader(UITableView tableView, nint section)
        {
            var header = tableView.DequeueReusableHeaderFooterView(headerCellIdentifier);

            if (header is IMvxBindable bindable)
                bindable.DataContext = ViewModel;

            return header;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var item = GetItemAt(indexPath);
            var cell = GetOrCreateCellFor(tableView, indexPath, item);
            cell.SelectionStyle = UITableViewCellSelectionStyle.None;

            if (item != null && cell is IMvxBindable bindable)
                bindable.DataContext = item;

            return cell;
        }

        protected override UITableViewCell GetOrCreateCellFor(UITableView tableView, NSIndexPath indexPath, object item)
            => tableView.DequeueReusableCell(cellIdentifier, indexPath);

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath) => 56;

        public override nfloat GetHeightForHeader(UITableView tableView, nint section) => headerHeight;
    }
}
