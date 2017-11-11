using CoreGraphics;
using UIKit;

namespace Toggl.Daneel.Extensions
{
    public static class ViewExtensions
    {
        public static void Resize(this UISwitch self, int height)
        {
            var scale = height / self.Frame.Height;
            self.Transform = CGAffineTransform.MakeScale(scale, scale);
        }
    }
}
