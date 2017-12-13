using UIKit;

namespace Toggl.Daneel.Services
{
    public interface ITopViewControllerProvider
    {
        UIViewController TopViewController { get; }
    }
}
