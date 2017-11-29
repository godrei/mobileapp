﻿using System;
using CoreGraphics;
using Foundation;
using MvvmCross.Plugins.Color.iOS;
using Toggl.Foundation.MvvmCross.Helper;
using UIKit;

namespace Toggl.Daneel
{
    public sealed class TokenTextAttachment : NSTextAttachment
    {
        private const int lineHeight = 24;
        private const int tokenHeight = 22;
        private const int tokenPadding = 6;
        private const int interTokenSpacing = 6;
        private const float tokenCornerRadius = 6.0f;
        private const int tokenVerticallOffset = (lineHeight - tokenHeight) / 2;

        private const int dotPadding = 6;
        private const int dotDiameter = 4;
        private const int dotRadius = dotDiameter / 2;
        private const int dotYOffset = (lineHeight / 2) - dotRadius;

        private static readonly UIColor borderColor = Color.StartTimeEntry.TokenBorder.ToNativeColor();

        public TokenTextAttachment(NSAttributedString tokenStringToDraw, 
            nfloat textVerticalOffset, nfloat fontDescender)
        {
            var size = new CGSize(tokenStringToDraw.Size.Width + interTokenSpacing + (tokenPadding * 2), lineHeight);
            UIGraphics.BeginImageContextWithOptions(size, false, 0.0f);
            using (var context = UIGraphics.GetCurrentContext())
            {
                var tokenPath = UIBezierPath.FromRoundedRect(new CGRect(
                    x: interTokenSpacing,
                    y: tokenVerticallOffset,
                    width: size.Width - interTokenSpacing,
                    height: tokenHeight
                ), tokenCornerRadius);
                context.AddPath(tokenPath.CGPath);
                context.SetStrokeColor(borderColor.CGColor);
                context.StrokePath();

                tokenStringToDraw.DrawString(new CGPoint(interTokenSpacing + tokenPadding, textVerticalOffset));

                var image = UIGraphics.GetImageFromCurrentImageContext();
                UIGraphics.EndImageContext();
                Image = image;
            }

            FontDescender = fontDescender;
        }

        public TokenTextAttachment(NSAttributedString projectStringToDraw, 
            nfloat textVerticalOffset, UIColor projectColor, nfloat fontDescender)
        {
            const int circleWidth = dotDiameter + dotPadding;
            var totalWidth = projectStringToDraw.Size.Width + circleWidth + interTokenSpacing + (tokenPadding * 2);
            var size = new CGSize(totalWidth, lineHeight);
            UIGraphics.BeginImageContextWithOptions(size, false, 0.0f);
            using (var context = UIGraphics.GetCurrentContext())
            {
                var tokenPath = UIBezierPath.FromRoundedRect(new CGRect(
                    x: interTokenSpacing,
                    y: tokenVerticallOffset,
                    width: totalWidth - interTokenSpacing,
                    height: tokenHeight
                ), tokenCornerRadius);
                context.AddPath(tokenPath.CGPath);
                context.SetFillColor(projectColor.ColorWithAlpha(0.12f).CGColor);
                context.FillPath();

                var dot = UIBezierPath.FromRoundedRect(new CGRect(
                    x: dotPadding + interTokenSpacing,
                    y: dotYOffset,
                    width: dotDiameter,
                    height: dotDiameter
                ), dotRadius);
                context.AddPath(dot.CGPath);
                context.SetFillColor(projectColor.CGColor);
                context.FillPath();

                projectStringToDraw.DrawString(new CGPoint(circleWidth + interTokenSpacing + tokenPadding, textVerticalOffset));

                var image = UIGraphics.GetImageFromCurrentImageContext();
                UIGraphics.EndImageContext();
                Image = image;
            }

            FontDescender = fontDescender;
        }

        public nfloat FontDescender { get; private set; }

        public override CGRect GetAttachmentBounds(NSTextContainer textContainer,
            CGRect proposedLineFragment, CGPoint glyphPosition, nuint characterIndex)
        {
            var rect = base.GetAttachmentBounds(textContainer,
                proposedLineFragment, glyphPosition, characterIndex);

            rect.Y = FontDescender;
            return rect;
        }
    }
}
