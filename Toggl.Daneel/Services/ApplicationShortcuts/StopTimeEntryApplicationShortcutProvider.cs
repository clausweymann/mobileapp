using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
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
        private readonly ITogglDataSource dataSource;
        private readonly ITimeService timeService;

        private readonly UIApplicationShortcutIcon icon = UIApplicationShortcutIcon.FromType(UIApplicationShortcutIconType.Pause);

        //TODO: com.toggl.daneel.stopTimeEntry ?
        public string ShortcutType { get; } = "StopTimeEntry";

        private Subject<IEnumerable<UIApplicationShortcutItem>> shortcutsSubject
            = new Subject<IEnumerable<UIApplicationShortcutItem>>();
        public IObservable<IEnumerable<UIApplicationShortcutItem>> Shortcuts { get; }

        public IApplicationShortcutHandler Handler { get; }

        private IDisposable currentTimeEntryDisposable;

        public StopTimeEntryApplicationShortcutProvider(
            ITogglDataSource dataSource, ITimeService timeService)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));

            this.dataSource = dataSource;
            this.timeService = timeService;

            Shortcuts = shortcutsSubject.AsObservable();
            Handler = new StopTimeEntryApplicationShortcutHandler(dataSource, timeService);

            currentTimeEntryDisposable = dataSource
                .TimeEntries
                .CurrentlyRunningTimeEntry
                .Subscribe(onCurrentlyRunningTimeEntry);
        }

        private void onCurrentlyRunningTimeEntry(IDatabaseTimeEntry timeEntry)
        {
            if (timeEntry == null)
            {
                shortcutsSubject.OnNext(null);
                return;
            }

            //UIApplicationShortcutItem can't be created/modified from non-UI thread
            UIApplication.SharedApplication.InvokeOnMainThread(() =>
            {
                var shortcut = new UIApplicationShortcutItem(
                    ShortcutType,
                    "Stop the fucking entry",
                    $"{timeEntry.Description} {timeEntry.Project?.Name}",
                    icon,
                    null
                );
                shortcutsSubject.OnNext(new[] { shortcut });
            });
        }

        public async Task Handle(UIApplicationShortcutItem shortcut)
            => await dataSource.TimeEntries.Stop(timeService.CurrentDateTime);
    }
}
