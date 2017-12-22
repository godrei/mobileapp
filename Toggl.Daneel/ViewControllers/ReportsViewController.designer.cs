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
		UIKit.UIView CalendarContainer { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint CalendarHeightConstraint { get; set; }

		[Outlet]
		UIKit.UITableView ReportsTableView { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint TopCalendarConstraint { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint TopConstraint { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (CalendarContainer != null) {
				CalendarContainer.Dispose ();
				CalendarContainer = null;
			}

			if (CalendarHeightConstraint != null) {
				CalendarHeightConstraint.Dispose ();
				CalendarHeightConstraint = null;
			}

			if (ReportsTableView != null) {
				ReportsTableView.Dispose ();
				ReportsTableView = null;
			}

			if (TopConstraint != null) {
				TopConstraint.Dispose ();
				TopConstraint = null;
			}

			if (TopCalendarConstraint != null) {
				TopCalendarConstraint.Dispose ();
				TopCalendarConstraint = null;
			}
		}
	}
}
