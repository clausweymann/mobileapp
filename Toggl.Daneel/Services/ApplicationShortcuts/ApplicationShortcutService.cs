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

namespace Toggl.Daneel.Services
{
    public sealed class ApplicationShortcutService
    {
        private readonly List<IApplicationShortcutProvider> shortcutProviders;
        private readonly Dictionary<string, IApplicationShortcutHandler> shortcutHandlers;

        private readonly CompositeDisposable disposeBag = new CompositeDisposable();

        public ApplicationShortcutService(
            ITopViewControllerProvider topViewControllerProvider,
            IMvxNavigationService navigationService,
            ITogglDataSource dataSource,
            ITimeService timeService)
        {
            Ensure.Argument.IsNotNull(topViewControllerProvider, nameof(topViewControllerProvider));
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));

            shortcutProviders = new List<IApplicationShortcutProvider>
            {
                new StopTimeEntryApplicationShortcutProvider(dataSource, timeService)
            };

            shortcutHandlers = shortcutProviders.ToDictionary(
                shortcut => shortcut.ShortcutType,
                shortcut => shortcut.Handler);
            
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

        //TODO: smarter updating
        private void onShortcuts(IEnumerable<UIApplicationShortcutItem> shortcuts)
        {
            UIApplication.SharedApplication.ShortcutItems
                = shortcuts?.ToArray() ?? new UIApplicationShortcutItem[0];
        }

        public Task Handle(UIApplicationShortcutItem shortcut)
            => shortcutHandlers[shortcut.Type].Handle(shortcut);
    }
}
