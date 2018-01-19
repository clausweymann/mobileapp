using Toggl.Foundation.DataSources;
using System.Linq;
using Toggl.Multivac;
using System;
using System.Reactive.Linq;
using UIKit;
using Toggl.Foundation;
using System.Collections.Generic;
using Toggl.Daneel.Services.ApplicationShortcuts;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using MvvmCross.Core.Navigation;
using Toggl.Foundation.Suggestions;
using Toggl.PrimeRadiant;
using Toggl.Foundation.Login;

namespace Toggl.Daneel.Services
{
    public sealed class ApplicationShortcutService
    {
        private readonly List<IApplicationShortcutProvider> shortcutProviders;
        private readonly Dictionary<string, IApplicationShortcutHandler> shortcutHandlers;

        private readonly CompositeDisposable disposeBag = new CompositeDisposable();

        private readonly IReadOnlyDictionary<string, int> priorities;

        public ApplicationShortcutService(
            ITopViewControllerProvider topViewControllerProvider,
            IMvxNavigationService navigationService,
            ITogglDataSource dataSource,
            ITimeService timeService,
            ITogglDatabase database)
        {
            Ensure.Argument.IsNotNull(topViewControllerProvider, nameof(topViewControllerProvider));
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));
            Ensure.Argument.IsNotNull(database, nameof(database));

            UIApplication.SharedApplication.ShortcutItems = new UIApplicationShortcutItem[0];

            shortcutProviders = new List<IApplicationShortcutProvider>
            {
                //new ReportsApplicationShortcutProvider(loginManager, dataSource),
                new StopTimeEntryApplicationShortcutProvider(dataSource, timeService),
                new TimeEntrySuggestionsApplicationShortcutProvider(
                    new SuggestionProviderContainer(
                        new MostRecentTimeEntrySuggestionProvider(dataSource),
                        new MostUsedTimeEntrySuggestionProvider(database, timeService, 3)
                    )
                )
            };

            priorities = new Dictionary<string, int>
            {
                //{nameof(ReportsApplicationShortcutProvider), 0},
                {nameof(StopTimeEntryApplicationShortcutProvider), 1},
                {nameof(TimeEntrySuggestionsApplicationShortcutProvider), 2}
            };
            
            shortcutHandlers = shortcutProviders.ToDictionary(
                shortcut => shortcut.ShortcutType,
                shortcut => shortcut.Handler);

            //The static stuff
            shortcutHandlers.Add(
                "Reports",
                new ReportsApplicationShortcutHandler(
                    topViewControllerProvider, navigationService, dataSource));

            foreach (var shortcut in shortcutProviders)
            {
                var subscription = shortcut.Shortcuts.Subscribe(onShortcuts);
                disposeBag.Add(subscription);
            }
        }

        private void onShortcuts(ShortcutCollection shortcutsCollection)
        {
            var shortcutList = UIApplication.SharedApplication.ShortcutItems.ToList();

            shortcutList.RemoveAll(shortcut => shortcut.Type == shortcutsCollection.Type);

            if (shortcutsCollection.HasShortcuts)
                shortcutList.AddRange(shortcutsCollection.Shortcuts);

            UIApplication.SharedApplication.ShortcutItems = shortcutList
                .OrderBy(shortcuts => shortcuts.Type, new TheComparer<string>(priorities)).ToArray();
            
        }

        public Task Handle(UIApplicationShortcutItem shortcut)
            => shortcutHandlers[shortcut.Type].Handle(shortcut);

        private class TheComparer<T> : IComparer<T>
        {
            private readonly IReadOnlyDictionary<T, int> priorities;

            public TheComparer(IReadOnlyDictionary<T, int> priorities)
            {
                this.priorities = priorities;
            }

            public int Compare(T x, T y)
            {
                return priorities[x].CompareTo(priorities[y]);
            }
        }
    }
}
