using System;
using Toggl.Multivac;

namespace Toggl.Foundation.Autocomplete.Suggestions
{
    public struct NoEntityInfoMessage
    {
        public string Text { get; }

        public string ImageResource { get; }

        public int? ImageResourceIndex { get; }

        public NoEntityInfoMessage(
            string text, string imageResource, int? imageResourceIndex)
        {
            Ensure.Argument.IsNotNull(text, nameof(text));

            if (!string.IsNullOrEmpty(imageResource) && imageResourceIndex == null)
                throw new ArgumentNullException($"{nameof(imageResourceIndex)} must not be null, when {nameof(imageResource)} is set");

            Text = text;
            ImageResource = imageResource;
            ImageResourceIndex = imageResourceIndex;
        }
    }
}
