using System;
using CoreGraphics;
using MvvmCross.Binding;
using MvvmCross.Binding.Bindings.Target;
using UIKit;

namespace Toggl.Daneel.Binding
{
    public sealed class ScrollViewAnimatedCurrentPageTargetBinding
        : MvxTargetBinding<UIScrollView, int>
    {
        public const string BindingName = "AnimatedCurrentPage";

        public override MvxBindingMode DefaultMode => MvxBindingMode.TwoWay;

        public ScrollViewAnimatedCurrentPageTargetBinding(UIScrollView target)
            : base(target) 
        {
            Target.DecelerationEnded += onDecelerationEnded;
        }

        protected override void SetValue(int value)
        {
            var scrollPoint = new CGPoint(Target.Frame.Size.Width * value, 0);
            Target.SetContentOffset(scrollPoint, true);
        }

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);

            if (!isDisposing) return;
            Target.DecelerationEnded -= onDecelerationEnded;
        }

        private void onDecelerationEnded(object sender, EventArgs e)
        {
            var newPage = (int)(Target.ContentOffset.X / Target.Frame.Width);
            FireValueChanged(newPage);
        }
    }
}
