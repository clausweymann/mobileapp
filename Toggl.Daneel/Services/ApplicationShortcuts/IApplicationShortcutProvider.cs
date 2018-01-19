using System;

namespace Toggl.Daneel.Services.ApplicationShortcuts
{
    public interface IApplicationShortcutProvider
    {
        string ShortcutType { get; }

        IObservable<ShortcutCollection> Shortcuts { get; }

        IApplicationShortcutHandler Handler { get; }
    }
}
