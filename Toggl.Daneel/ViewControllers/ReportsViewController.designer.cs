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
	[Register ("ReportsViewController")]
	partial class ReportsViewController
	{
		[Outlet]
		UIKit.NSLayoutConstraint CalendarContainerHeightConstraint { get; set; }

		[Outlet]
		UIKit.UIView CalendarContainerView { get; set; }

		[Outlet]
		UIKit.UIView EmptyStateView { get; set; }

		[Outlet]
		UIKit.UIImageView ImageView { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint TopConstraint { get; set; }

		[Outlet]
		UIKit.UILabel TotalDurationLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (CalendarContainerView != null) {
				CalendarContainerView.Dispose ();
				CalendarContainerView = null;
			}

			if (EmptyStateView != null) {
				EmptyStateView.Dispose ();
				EmptyStateView = null;
			}

			if (ImageView != null) {
				ImageView.Dispose ();
				ImageView = null;
			}

			if (TopConstraint != null) {
				TopConstraint.Dispose ();
				TopConstraint = null;
			}

			if (TotalDurationLabel != null) {
				TotalDurationLabel.Dispose ();
				TotalDurationLabel = null;
			}

			if (CalendarContainerHeightConstraint != null) {
				CalendarContainerHeightConstraint.Dispose ();
				CalendarContainerHeightConstraint = null;
			}
		}
	}
}
