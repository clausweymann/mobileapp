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
            
            var result = new NSMutableAttributedString(noEntityInfoMessage.Text);
            var rangeToBeReplaced = new NSRange(
                noEntityInfoMessage
                    .Text
                    .IndexOf(noEntityInfoMessage.CharacterToReplace.Value),
                len: 1);
            var imageAttachment = noEntityInfoMessage
                .ImageResource
                .GetAttachmentString(
                    fontCapHeight,
                    UIImageRenderingMode.AlwaysOriginal);
            
            result.Replace(rangeToBeReplaced, imageAttachment);

            return result;
        }
    }
}
