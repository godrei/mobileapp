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
	[Register ("ReportsCalendarViewController")]
	partial class ReportsCalendarViewController
	{
		[Outlet]
		UIKit.UICollectionView CalendarCollectionView { get; set; }

		[Outlet]
		UIKit.UILabel CurrentMonthLabel { get; set; }

		[Outlet]
		UIKit.UILabel CurrentYearLabel { get; set; }

		[Outlet]
		UIKit.UICollectionView QuickSelectCollectionView { get; set; }

		[Outlet]
		UIKit.UICollectionViewFlowLayout QuickSelectCollectionViewLayout { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (CalendarCollectionView != null) {
				CalendarCollectionView.Dispose ();
				CalendarCollectionView = null;
			}

			if (CurrentMonthLabel != null) {
				CurrentMonthLabel.Dispose ();
				CurrentMonthLabel = null;
			}

			if (CurrentYearLabel != null) {
				CurrentYearLabel.Dispose ();
				CurrentYearLabel = null;
			}

			if (QuickSelectCollectionView != null) {
				QuickSelectCollectionView.Dispose ();
				QuickSelectCollectionView = null;
			}

			if (QuickSelectCollectionViewLayout != null) {
				QuickSelectCollectionViewLayout.Dispose ();
				QuickSelectCollectionViewLayout = null;
			}
		}
	}
}
