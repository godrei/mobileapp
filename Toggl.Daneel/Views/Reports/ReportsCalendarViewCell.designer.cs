// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Toggl.Daneel.Views
{
	[Register ("ReportsCalendarViewCell")]
	partial class ReportsCalendarViewCell
	{
		[Outlet]
		Toggl.Daneel.Views.RoundedView BackgroundView { get; set; }

		[Outlet]
		UIKit.UILabel Text { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (Text != null) {
				Text.Dispose ();
				Text = null;
			}

			if (BackgroundView != null) {
				BackgroundView.Dispose ();
				BackgroundView = null;
			}
		}
	}
}
