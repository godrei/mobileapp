using System;
using System.Threading.Tasks;
using CoreGraphics;
using Foundation;
using MvvmCross.Core.ViewModels;
using MvvmCross.Plugins.Color.iOS;
using Toggl.Daneel.Extensions;
using Toggl.Foundation;
using Toggl.Foundation.MvvmCross.Helper;
using Toggl.Foundation.Sync;
using UIKit;

namespace Toggl.Daneel.Views
{
    [Register(nameof(MainScrollView))]
    public sealed class MainScrollView : UIScrollView
    {
        internal const float SyncStateViewHeight = 26;
        private static readonly float scrollThreshold = SyncStateViewHeight + (SyncStateViewHeight / 2);

        private readonly UIColor syncingColor = Color.Main.Syncing.ToNativeColor();
        private readonly UIColor syncFailedColor = Color.Main.SyncFailed.ToNativeColor();
        private readonly UIColor offlineColor = Color.Main.Offline.ToNativeColor();
        private readonly UIColor syncCompletedColor = Color.Main.SyncCompleted.ToNativeColor();

        private SyncProgress syncProgress;
        private bool needsRefresh;
        private bool shouldCalculateOnDeceleration;
        private bool isSyncing => syncProgress == SyncProgress.Syncing;

        public SyncProgress SyncProgress
        {
            get => syncProgress;
            set
            {
                if (value == syncProgress) return;
                syncProgress = value;
                updateViews(value);
            }
        }

        public UIView SyncStateView { get; set; }

        public UILabel SyncStateLabel { get; set; }

        public IMvxCommand RefreshCommand { get; set; }

        public MainScrollView(IntPtr handle)
            : base(handle)
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            Scrolled += onScrolled;
            DraggingEnded += onDragEnded;
            DecelerationEnded += onDecelerationEnded;
            ShouldScrollToTop = shouldScrollToTop;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposing) return;

            Scrolled -= onScrolled;
            DraggingEnded -= onDragEnded;
            DecelerationEnded -= onDecelerationEnded;
        }

        private void onScrolled(object sender, EventArgs e)
        {
            if (!Dragging) return;

            var offset = ContentOffset.Y;
            if (offset >= 0) return;

            var needsMorePulling = Math.Abs(offset) < scrollThreshold;
            needsRefresh = !needsMorePulling;

            if (isSyncing) return;

            Animate(Animation.Timings.EnterTiming, () =>
                SyncStateLabel.Text = needsMorePulling ? Resources.PullDownToRefresh : Resources.ReleaseToRefresh);
        }

        private void onDragEnded(object sender, DraggingEventArgs e)
        {
            var offset = ContentOffset.Y;
            if (offset >= 0) return;

            shouldCalculateOnDeceleration = e.Decelerate;

            if (shouldCalculateOnDeceleration) return;
            refreshIfNeeded();
        }

        private void onDecelerationEnded(object sender, EventArgs e)
        {
            if (!shouldCalculateOnDeceleration) return;
            refreshIfNeeded();
        }

        private void refreshIfNeeded()
        {
            if (!needsRefresh)
            {
                scrollToIfInFirstPage(CGPoint.Empty);
                return;
            }

            needsRefresh = false;
            shouldCalculateOnDeceleration = false;

            if (isSyncing)
            {
                scrollToIfInFirstPage(new CGPoint(0, -SyncStateViewHeight));
                return;
            }

            RefreshCommand.Execute();
        }

        private async void updateViews(SyncProgress value)
        {
            NSAttributedString text;
            UIColor backgroundColor;
            bool hideIndicator = true;

            switch (value)
            {
                case SyncProgress.Unknown:
                    return;

                case SyncProgress.Syncing:
                    text = new NSAttributedString(Resources.Syncing);
                    backgroundColor = syncingColor;
                    hideIndicator = false;
                    break;

                case SyncProgress.OfflineModeDetected:
                    text = new NSAttributedString(Resources.Offline);
                    backgroundColor = offlineColor;
                    hideIndicator = false;
                    break;

                case SyncProgress.Synced:
                    text = Resources.SyncCompleted.EndingWithTick(SyncStateLabel.Font.CapHeight);
                    backgroundColor = syncCompletedColor;
                    break;

                case SyncProgress.Failed:
                    text = new NSAttributedString(Resources.SyncFailed);
                    backgroundColor = syncFailedColor;
                    break;

                default:
                    throw new ArgumentException(nameof(value));
            }

            Animate(Animation.Timings.EnterTiming, () =>
            {
                SyncStateLabel.AttributedText = text;
                SyncStateView.BackgroundColor = backgroundColor;
            });

            scrollToIfInFirstPage(new CGPoint(0, -SyncStateViewHeight));

            if (!hideIndicator) return;

            await hideSyncIndicator();
        }

        private async Task hideSyncIndicator()
        {
            await Task.Delay(Animation.Timings.HideSyncStateViewDelay);
            scrollToIfInFirstPage(CGPoint.Empty);
        }

        private void scrollToIfInFirstPage(CGPoint offset)
        {
            if (ContentOffset.Y > 0) return;

            SetContentOffset(offset, true);
        }

        private bool shouldScrollToTop(UIScrollView scrollView)
        {
            scrollView.SetContentOffset(CGPoint.Empty, true);
            return false;
        }
    }
}
