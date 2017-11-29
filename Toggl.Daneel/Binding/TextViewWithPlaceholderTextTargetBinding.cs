using System;
using MvvmCross.Binding;
using MvvmCross.Binding.Bindings.Target;
using Toggl.Daneel.Views;

namespace Toggl.Daneel.Binding
{
    public sealed class TextViewWithPlaceholderTextTargetBinding : MvxTargetBinding<TextViewWithPlaceholder, string>
    {
        public const string BindingName = "Text";

        public override MvxBindingMode DefaultMode => MvxBindingMode.TwoWay;

        public TextViewWithPlaceholderTextTargetBinding(TextViewWithPlaceholder target)
            : base(target)
        {
            target.TextChanged += onTextChanged;
        }

        protected override void SetValue(string value)
        {
            Target.Text = value;
        }

        private void onTextChanged(object sender, EventArgs e)
            => FireValueChanged(Target.Text);

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);

            if (!isDisposing)
                return;

            Target.TextChanged -= onTextChanged;
        }
    }
}
