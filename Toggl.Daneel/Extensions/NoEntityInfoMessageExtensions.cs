using System;
using Foundation;
using Toggl.Foundation.Autocomplete.Suggestions;
using UIKit;

namespace Toggl.Daneel.Extensions
{
    public static class NoEntityInfoMessageExtensions
    {
        public static NSAttributedString ToAttributedString(
            this NoEntityInfoMessage noEntityInfoMessage,
            nfloat fontCapHeight)
        {
            if (string.IsNullOrEmpty(noEntityInfoMessage.ImageResource))
                return new NSAttributedString(noEntityInfoMessage.Text);

            var textBeginning = noEntityInfoMessage
                .Text
                .Substring(0, noEntityInfoMessage.ImageResourceIndex.Value);
            var textEnd = noEntityInfoMessage
                .Text
                .Substring(noEntityInfoMessage.ImageResourceIndex.Value);
            
            var result = new NSMutableAttributedString(textBeginning);
            var imageAttachment = noEntityInfoMessage
                .ImageResource
                .GetAttachmentString(
                    fontCapHeight,
                    UIImageRenderingMode.AlwaysOriginal);
            result.Append(imageAttachment);
            result.Append(new NSAttributedString(textEnd));
            return result;
        }
    }
}
