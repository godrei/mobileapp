using System;
using MvvmCross.Binding;
using MvvmCross.Binding.Bindings.Target;
using Toggl.Daneel.Extensions;
using Toggl.Foundation.MvvmCross.Helper;
using UIKit;

namespace Toggl.Daneel.Binding
{
    public sealed class LayoutConstraintAnimatedConstantTargetBinding
        : MvxTargetBinding<NSLayoutConstraint, nfloat>
    {
        public const string BindingName = "AnimatedConstant";

        public override MvxBindingMode DefaultMode => MvxBindingMode.OneWay;

        public LayoutConstraintAnimatedConstantTargetBinding(NSLayoutConstraint target)
            : base(target)
        {
        }

        protected override void SetValue(nfloat value)
        {
            Target.Constant = value;
            AnimationExtensions.Animate(
                Animation.Timings.EnterTiming,
                Animation.Curves.SharpCurve,
                () => ((UIView)Target.FirstItem).Superview.LayoutSubviews()
            );
        }
    }
}
