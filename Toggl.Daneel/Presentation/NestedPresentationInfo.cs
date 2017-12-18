using System;
using Toggl.Multivac;
using UIKit;

namespace Toggl.Daneel.Presentation
{
    public interface INestedPresentationInfo
    {
        UIViewController ViewController { get; }
        UIView Container { get; }
    }

    public sealed class NestedPresentationInfo<T> : INestedPresentationInfo
        where T : UIViewController
    {
        private readonly Func<T> getViewController;
        private readonly Func<T, UIView> getContainer;

        public T ViewController => getViewController();

        public UIView Container => getContainer(ViewController);

        UIViewController INestedPresentationInfo.ViewController => ViewController;

        public NestedPresentationInfo(
            Func<T> getViewController,
            Func<T, UIView> getContainer)
        {
            Ensure.Argument.IsNotNull(getViewController, nameof(getViewController));
            Ensure.Argument.IsNotNull(getContainer, nameof(getContainer));

            this.getViewController = getViewController;
            this.getContainer = getContainer;
        }
    }
}
