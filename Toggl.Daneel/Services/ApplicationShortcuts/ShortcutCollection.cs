using System.Collections.Generic;
using System.Linq;
using UIKit;

namespace Toggl.Daneel.Services.ApplicationShortcuts
{
    public sealed class ShortcutCollection
    {
        public string Type { get; }

        public List<UIApplicationShortcutItem> Shortcuts { get; }

        public bool HasShortcuts => Shortcuts.Any();

        public ShortcutCollection(
            string type,
            IEnumerable<UIApplicationShortcutItem> shortcuts)
        {
            Type = type;
            Shortcuts = shortcuts?.ToList() ?? new List<UIApplicationShortcutItem>();
        }

        public static ShortcutCollection Empty(string type)
            => new ShortcutCollection(type, null);
    }
}
