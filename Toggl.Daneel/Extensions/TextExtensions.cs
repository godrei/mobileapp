using System.Linq;
using System.Text;
using CoreGraphics;
using Foundation;
using MvvmCross.Platform.UI;
using MvvmCross.Plugins.Color.iOS;
using Toggl.Daneel.Views;
using Toggl.Foundation.Autocomplete;
using Toggl.Foundation.MvvmCross.Helper;
using UIKit;

namespace Toggl.Daneel.Extensions
{
    public static class TextExtensions
    {
        private const string space = "\u00A0";
        private const string addIcon = "icAdd";
        private static readonly NSParagraphStyle paragraphStyle;
        private static readonly UIColor strokeColor = Color.StartTimeEntry.ProjectTokenBorder.ToNativeColor();

        static TextExtensions()
        {
            paragraphStyle = new NSMutableParagraphStyle
            {
                MinimumLineHeight = 24,
                MaximumLineHeight = 24
            };
        }

        public static string GetDescription(this UITextView self, TextFieldInfo info)
        {
            var result = self.Text;

            var projectText = info.PaddedProjectAndTaskName();
            if (!string.IsNullOrEmpty(projectText))
                result = result.Replace(projectText, "");

            foreach (var tag in info.Tags)
                result = result.Replace(PaddedTagName(tag.Name), "");
           
            return result;
        }

        public static string PaddedProjectAndTaskName(this TextFieldInfo self)
        {
            if (string.IsNullOrEmpty(self.ProjectName)) return "";

            var builder = new StringBuilder($"      {self.ProjectName}");
            if (self.TaskId != null)
                builder.Append($": {self.TaskName}");
            builder.Append("   ");
            builder.Replace(" ", space);

            return builder.ToString();
        }

        public static string TagsText(this TextFieldInfo self)
            => self.Tags.Length == 0 ? "" :
                   self.Tags.Aggregate(
                       new StringBuilder(""), 
                       (builder, tag) => builder.Append(PaddedTagName(tag.Name)))
                   .ToString()
                   .Replace(" ", space);

        public static string PaddedTagName(string tagName)
            => $"   {tagName}   ".Replace(" ", space);

        public static NSAttributedString GetAttributedText(this TextFieldInfo self)
        {
            var projectName = self.PaddedProjectAndTaskName();
            var tags = self.TagsText();
            var fullText = $"{self.Text}{projectName}{tags}";
            var result = new NSMutableAttributedString(fullText);
            var baselineOffset = string.IsNullOrEmpty(self.Text) ? 5 : 3;

            result.AddAttributes(new UIStringAttributes
            {
                ParagraphStyle = paragraphStyle,
                Font = UIFont.SystemFontOfSize(16, UIFontWeight.Regular),
            }, new NSRange(0, self.Text.Length));

            if (!string.IsNullOrEmpty(self.ProjectColor))
            {
                var color = MvxColor.ParseHexString(self.ProjectColor).ToNativeColor();

                var attributes = new UIStringAttributes
                {
                    ForegroundColor = color,
                    StrokeColor = strokeColor,
                    BaselineOffset = baselineOffset,
                    ParagraphStyle = paragraphStyle,
                    Font = UIFont.SystemFontOfSize(12, UIFontWeight.Regular),
                };
                attributes.Dictionary[TimeEntryTagsTextView.RoundedBackground] = color.ColorWithAlpha(0.12f);

                result.AddAttributes(attributes, new NSRange(self.Text.Length, projectName.Length));
            }

            if (!string.IsNullOrEmpty(tags))
            {
                var startingPosition = self.Text.Length + projectName.Length;
                
                for (int i = 0; i < self.Tags.Length; i++)
                {
                    var tagLength = self.Tags[i].Name.Length + 6;

                    var attributes = new UIStringAttributes
                    {
                        BaselineOffset = baselineOffset,
                        ParagraphStyle = paragraphStyle,
                        Font = UIFont.SystemFontOfSize(12, UIFontWeight.Regular),
                    };
                    attributes.Dictionary[TimeEntryTagsTextView.TagIndex] = new NSNumber(i);
                    attributes.Dictionary[TimeEntryTagsTextView.RoundedBorders] = strokeColor;
                    result.AddAttributes(attributes, new NSRange(startingPosition, tagLength));

                    startingPosition += tagLength;
                }
            }

            return result;
        }

        public static NSAttributedString PrependWithAddIcon(this string self, double fontHeight)
        {
            var textColor = Color.StartTimeEntry.Placeholder.ToNativeColor();
            var addIconColor = Color.StartTimeEntry.AddIconColor.ToNativeColor();

            var emptyTextString = new NSMutableAttributedString($" {self}");
            emptyTextString.AddAttribute(UIStringAttributeKey.ForegroundColor, textColor, new NSRange(0, emptyTextString.Length));

            var result = addIcon.GetAttachmentString(fontHeight);
            // We need a space before the attachment for colors to work. The Zero-width space character does not work. 
            // The space makes the left margin too big. This is here so the space does not appear. 
            result.AddAttribute(UIStringAttributeKey.Font, UIFont.SystemFontOfSize(0), new NSRange(0, 1));
            result.AddAttribute(UIStringAttributeKey.ForegroundColor, addIconColor, new NSRange(0, result.Length));
            result.Append(emptyTextString);

            return result;
        }

        public static NSAttributedString EndingWithTick(this string self, double fontHeight)
        {
            var tick = GetAttachmentString("icDoneSmall", fontHeight);

            var range = new NSRange(0, 1);
            var attributes = new UIStringAttributes { ForegroundColor = UIColor.White };
            tick.AddAttributes(attributes, range);

            var result = new NSMutableAttributedString(self);
            result.Append(tick);

            return result;
        }

        public static NSMutableAttributedString GetAttachmentString(this string imageName, double fontHeight)
        {
            var attachment = new NSTextAttachment
            {
                Image = UIImage.FromBundle(imageName)
                               .ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate)
            };

            var imageSize = attachment.Image.Size;
            var y = (fontHeight - imageSize.Height) / 2;
            attachment.Bounds = new CGRect(0, y, imageSize.Width, imageSize.Height);

            //There neeeds to be a space before the dot, otherwise the colors don't work
            var result = new NSMutableAttributedString(" ");
            var attachmentString = NSAttributedString.FromAttachment(attachment);
            result.Append(attachmentString);

            return result;
        }
    }
}
