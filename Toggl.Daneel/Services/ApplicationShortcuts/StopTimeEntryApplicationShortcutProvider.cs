using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Toggl.Foundation;
using Toggl.Foundation.DataSources;
using Toggl.Multivac;
using Toggl.PrimeRadiant.Models;
using UIKit;
using static Toggl.Multivac.Extensions.ObservableExtensions;

namespace Toggl.Daneel.Services.ApplicationShortcuts
{
    public class StopTimeEntryApplicationShortcutProvider : IApplicationShortcutProvider
    {
        private readonly UIApplicationShortcutIcon icon
            = UIApplicationShortcutIcon.FromType(UIApplicationShortcutIconType.Pause);

        //TODO: com.toggl.daneel.stopTimeEntry ?
        public string ShortcutType { get; } = nameof(StopTimeEntryApplicationShortcutProvider);

        private Subject<ShortcutCollection> shortcutsSubject
            = new Subject<ShortcutCollection>();

        public IObservable<ShortcutCollection> Shortcuts { get; }

        public IApplicationShortcutHandler Handler { get; }

        private IDisposable currentTimeEntryDisposable;

        public StopTimeEntryApplicationShortcutProvider(
            ITogglDataSource dataSource, ITimeService timeService)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));

            Shortcuts = shortcutsSubject.AsObservable();
            Handler = new StopTimeEntryApplicationShortcutHandler(dataSource, timeService);

            currentTimeEntryDisposable = dataSource
                .TimeEntries
                .CurrentlyRunningTimeEntry
                .Subscribe(onCurrentlyRunningTimeEntry);

            Foundation.Login.ILoginManager l;
        }

        private void onCurrentlyRunningTimeEntry(IDatabaseTimeEntry timeEntry)
        {
            if (timeEntry == null)
            {
                shortcutsSubject.OnNext(new ShortcutCollection(ShortcutType, null));
                return;
            }

            //UIApplicationShortcutItem can't be created/modified from non-UI thread
            UIApplication.SharedApplication.InvokeOnMainThread(() =>
            {
                var shortcut = new UIApplicationShortcutItem(
                    ShortcutType,
                    Resources.AppShortcutStop,
                    $"{timeEntry.Description} {timeEntry.Project?.Name}",
                    icon,
                    null
                );
                shortcutsSubject.OnNext(new ShortcutCollection(ShortcutType, new[] { shortcut }));
            });
        }
    }
}
