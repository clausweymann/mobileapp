using System;
using Toggl.Multivac;

namespace Toggl.Foundation.Autocomplete.Suggestions
{
    public struct NoEntityInfoMessage
    {
        public string Text { get; }

        public string ImageResource { get; }

        public char? CharacterToReplace { get; }

        public NoEntityInfoMessage(
            string text, string imageResource, char? characterToReplace)
        {
            Ensure.Argument.IsNotNull(text, nameof(text));

            if (!string.IsNullOrEmpty(imageResource) && characterToReplace == null)
                throw new ArgumentNullException($"{nameof(characterToReplace)} must not be null, when {nameof(imageResource)} is set");

            Text = text;
            ImageResource = imageResource;
            CharacterToReplace = characterToReplace;
        }
    }
}
