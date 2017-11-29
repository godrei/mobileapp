using System;
using Foundation;
using MvvmCross.Platform.UI;
using MvvmCross.Plugins.Color.iOS;
using Toggl.Multivac.Extensions;
using Toggl.Foundation.Autocomplete;
using Toggl.Foundation.MvvmCross.Helper;
using UIKit;

namespace Toggl.Daneel.Extensions
{
    public static class TokenExtensions
    {
        private const int lineHeight = 24;
        private const int maxTextLength = 50;

        private static readonly nfloat textVerticalOffset;
        private static readonly NSParagraphStyle paragraphStyle;
        private static readonly UIStringAttributes tagAttributes;
        private static readonly UIFont tokenFont = UIFont.SystemFontOfSize(12, UIFontWeight.Regular);
        private static readonly UIFont regularFont = UIFont.SystemFontOfSize(16, UIFontWeight.Regular);

        public static readonly NSString Project = new NSString(nameof(Project));
        public static readonly NSString TagIndex = new NSString(nameof(TagIndex));

        static TokenExtensions()
        {
            paragraphStyle = new NSMutableParagraphStyle
            {
                MinimumLineHeight = lineHeight,
                MaximumLineHeight = lineHeight
            };

            tagAttributes = new UIStringAttributes
            {
                Font = tokenFont,
                ForegroundColor = Color.StartTimeEntry.TokenText.ToNativeColor()
            };

            textVerticalOffset = (lineHeight / 2) - (tokenFont.CapHeight / 2) - 3;
        }

        public static string GetDescription(this UITextView self) => self.Text.Replace("￼", "");

        public static NSAttributedString GetAttributedText(this TextFieldInfo self)
        {
            var result = new NSMutableAttributedString(self.Text, new UIStringAttributes
            {
                Font = regularFont,
                ParagraphStyle = paragraphStyle
            });

            addProjectAttachmentsIfNeeded(self, result);
            addTagAttachmentsIfNeeded(self, result);

            return result;
        }

        public static string TruncatedAt(this string self, int location)
            => self.Length <= location ? self : $"{self.UnicodeSafeSubstring(0, location - 3)}...";

        private static void addProjectAttachmentsIfNeeded(TextFieldInfo info, NSMutableAttributedString finalString)
        {
            if (string.IsNullOrEmpty(info.ProjectColor)) return;

            var color = MvxColor.ParseHexString(info.ProjectColor).ToNativeColor();

            var projectName = new NSAttributedString(info.ProjectName.TruncatedAt(maxTextLength), new UIStringAttributes
            {
                Font = tokenFont,
                ForegroundColor = color
            });

            var textAttachment = new TokenTextAttachment(projectName, textVerticalOffset, color, regularFont.Descender);
            var tokenString = new NSMutableAttributedString(NSAttributedString.FromAttachment(textAttachment));
            var attributes = new UIStringAttributes { ParagraphStyle = paragraphStyle };
            attributes.Dictionary[Project] = new NSObject();
            tokenString.AddAttributes(attributes, new NSRange(0, tokenString.Length));

            finalString.Append(tokenString);
        }

        private static void addTagAttachmentsIfNeeded(TextFieldInfo info, NSMutableAttributedString finalString)
        {
            if (info.Tags.Length == 0) return;

            for (var i = 0; i < info.Tags.Length; i++)
            {
                var tag = info.Tags[i];
                var tagName = new NSMutableAttributedString(tag.Name.TruncatedAt(maxTextLength), tagAttributes);

                var textAttachment = new TokenTextAttachment(tagName, textVerticalOffset, regularFont.Descender);

                var tokenString = new NSMutableAttributedString(NSAttributedString.FromAttachment(textAttachment));
                var attributes = new UIStringAttributes { ParagraphStyle = paragraphStyle };
                attributes.Dictionary[TagIndex] = new NSNumber(i);
                tokenString.AddAttributes(attributes, new NSRange(0, tokenString.Length));

                finalString.Append(tokenString);
            }
        }
    }
}
