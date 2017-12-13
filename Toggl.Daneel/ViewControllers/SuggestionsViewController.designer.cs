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
	[Register ("SuggestionsViewController")]
	partial class SuggestionsViewController
	{
		[Outlet]
		UIKit.UILabel NewUserDescriptionLabel { get; set; }

		[Outlet]
		UIKit.UILabel NewUserTitleLabel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UITableView SuggestionsTableView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UILabel WelcomeTextLabel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UILabel WelcomeTitleLabel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIView WelcomeView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (SuggestionsTableView != null) {
				SuggestionsTableView.Dispose ();
				SuggestionsTableView = null;
			}

			if (WelcomeTextLabel != null) {
				WelcomeTextLabel.Dispose ();
				WelcomeTextLabel = null;
			}

			if (WelcomeTitleLabel != null) {
				WelcomeTitleLabel.Dispose ();
				WelcomeTitleLabel = null;
			}

			if (WelcomeView != null) {
				WelcomeView.Dispose ();
				WelcomeView = null;
			}

			if (NewUserTitleLabel != null) {
				NewUserTitleLabel.Dispose ();
				NewUserTitleLabel = null;
			}

			if (NewUserDescriptionLabel != null) {
				NewUserDescriptionLabel.Dispose ();
				NewUserDescriptionLabel = null;
			}
		}
	}
}
