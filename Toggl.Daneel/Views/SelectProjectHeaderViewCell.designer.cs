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
	[Register ("SelectProjectHeaderViewCell")]
	partial class SelectProjectHeaderViewCell
	{
		[Outlet]
		UIKit.UIView TopSeparator { get; set; }

		[Outlet]
		UIKit.UILabel WorkspaceLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (WorkspaceLabel != null) {
				WorkspaceLabel.Dispose ();
				WorkspaceLabel = null;
			}

			if (TopSeparator != null) {
				TopSeparator.Dispose ();
				TopSeparator = null;
			}
		}
	}
}
