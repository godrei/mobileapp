using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Toggl.Daneel.Views
{
    [Register(nameof(ActivityIndicatorView))]
    public sealed class ActivityIndicatorView : UIImageView
    {
        private const float animationDuration = 0.5F;

        private string imageResource = "icLoader";
        public string ImageResource
        {
            get => imageResource;
            set => Image = UIImage.FromBundle(imageResource = value);
        }

        public ActivityIndicatorView(CGRect frame)
            : base (frame)
        {
            init();
        }

        public ActivityIndicatorView(IntPtr handle)
            : base(handle)
        {
        }

        private void init()
        {
            Image = UIImage.FromBundle(ImageResource);
            ContentMode = UIViewContentMode.Center;

            rotateView();
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            init();
        }

        private void rotateView()
        {
            Animate(animationDuration, 0, UIViewAnimationOptions.CurveLinear,
                () => Transform = CGAffineTransform.Rotate(Transform, (nfloat)Math.PI), rotateView);
        }
    }
}
