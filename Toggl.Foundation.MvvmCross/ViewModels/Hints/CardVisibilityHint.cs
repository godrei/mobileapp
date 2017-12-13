using MvvmCross.Core.ViewModels;

namespace Toggl.Foundation.MvvmCross.ViewModels.Hints
{
    public sealed class CardVisibilityHint : MvxPresentationHint
    {
        public bool Visible { get; }

        public CardVisibilityHint(bool visible)
        {
            Visible = visible;
        }
    }
}
