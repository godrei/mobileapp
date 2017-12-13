// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Toggl.Daneel.ViewControllers
{
	[Register ("TimeEntriesLogViewController")]
	partial class TimeEntriesLogViewController
	{
		[Outlet]
		UIKit.NSLayoutConstraint BottomConstraint { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIButton EmptyStateButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIImageView EmptyStateImageView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UILabel EmptyStateTextLabel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UILabel EmptyStateTitleLabel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIView EmptyStateView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UITableView TimeEntriesTableView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIImageView WelcomeImageView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (EmptyStateButton != null) {
				EmptyStateButton.Dispose ();
				EmptyStateButton = null;
			}

			if (EmptyStateImageView != null) {
				EmptyStateImageView.Dispose ();
				EmptyStateImageView = null;
			}

			if (EmptyStateTextLabel != null) {
				EmptyStateTextLabel.Dispose ();
				EmptyStateTextLabel = null;
			}

			if (EmptyStateTitleLabel != null) {
				EmptyStateTitleLabel.Dispose ();
				EmptyStateTitleLabel = null;
			}

			if (EmptyStateView != null) {
				EmptyStateView.Dispose ();
				EmptyStateView = null;
			}

			if (TimeEntriesTableView != null) {
				TimeEntriesTableView.Dispose ();
				TimeEntriesTableView = null;
			}

			if (WelcomeImageView != null) {
				WelcomeImageView.Dispose ();
				WelcomeImageView = null;
			}

			if (BottomConstraint != null) {
				BottomConstraint.Dispose ();
				BottomConstraint = null;
			}
		}
	}
}
