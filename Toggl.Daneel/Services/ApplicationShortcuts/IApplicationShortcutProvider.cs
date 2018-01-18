using System;
using System.Collections.Generic;
using UIKit;

namespace Toggl.Daneel.Services.ApplicationShortcuts
{
    public interface IApplicationShortcutProvider
    {
        string ShortcutType { get; }
        IObservable<IEnumerable<UIApplicationShortcutItem>> Shortcuts { get; }
        IApplicationShortcutHandler Handler { get; }
    }
}
