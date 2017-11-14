using System;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Binding.iOS.Views;
using Toggl.PrimeRadiant.Models;
using UIKit;

namespace Toggl.Daneel.Views
{
    public partial class WorkspaceViewCell : MvxTableViewCell
    {
        public static readonly NSString Key = new NSString(nameof(WorkspaceViewCell));
        public static readonly UINib Nib;

        static WorkspaceViewCell()
        {
            Nib = UINib.FromName(nameof(WorkspaceViewCell), NSBundle.MainBundle);
        }

        protected WorkspaceViewCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            this.DelayBind(() =>
            {
                var bindingSet = this.CreateBindingSet<WorkspaceViewCell, IDatabaseWorkspace>();

                bindingSet.Bind(NameLabel).To(vm => vm.Name);

                bindingSet.Apply();
            });
        }
    }
}
