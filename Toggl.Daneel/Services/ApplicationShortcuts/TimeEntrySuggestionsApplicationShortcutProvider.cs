using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using MvvmCross.Platform;
using Toggl.Foundation;
using Toggl.Foundation.DataSources;
using Toggl.Foundation.Suggestions;
using Toggl.Multivac;
using UIKit;
using IosSuggestion = Toggl.Daneel.Services.ApplicationShortcuts.Suggestion;

namespace Toggl.Daneel.Services.ApplicationShortcuts
{
    public class TimeEntrySuggestionsApplicationShortcutProvider
        : IApplicationShortcutProvider
    {
        private readonly ISuggestionProviderContainer suggestionProviderContainer;

        public string ShortcutType { get; } = nameof(TimeEntrySuggestionsApplicationShortcutProvider);

        public IObservable<ShortcutCollection> Shortcuts
            => createTheStuff();

        public IApplicationShortcutHandler Handler
            => new TimeEntrySuggestionsApplicationShortcutHandler(Mvx.Resolve<ITogglDataSource>(), Mvx.Resolve<ITimeService>());

        public TimeEntrySuggestionsApplicationShortcutProvider(
            ISuggestionProviderContainer suggestionProviderContainer)
        {
            Ensure.Argument.IsNotNull(suggestionProviderContainer, nameof(suggestionProviderContainer));

            this.suggestionProviderContainer = suggestionProviderContainer;
        }

        private IObservable<ShortcutCollection> createTheStuff()
        {
            return Observable.Create<ShortcutCollection>(sub =>
            {
                var subscription = suggestionProviderContainer
                    .Providers
                    .Select(provider => provider.GetSuggestions())
                    .Aggregate(Observable.Concat)
                    .Select(suggestion => new IosSuggestion(suggestion))
                    .Aggregate(
                        new List<IosSuggestion>(),
                        (acc, suggestion) => { acc.Add(suggestion); return acc; })
                    .Subscribe(suggestions => UIApplication.SharedApplication.InvokeOnMainThread(() =>
                    {
                        var shortcuts = new ShortcutCollection(ShortcutType, suggestions.Select(createShortcutItem));
                        sub.OnNext(shortcuts);
                        sub.OnCompleted();
                    }));

                return subscription;
            });
        }

        private UIApplicationShortcutItem createShortcutItem(IosSuggestion suggestion)
            => new UIApplicationShortcutItem(
                ShortcutType,
                Resources.AppShortcutStart,
                $"{suggestion.Description} {suggestion.ProjectName}",
                UIApplicationShortcutIcon.FromType(UIApplicationShortcutIconType.Play),
                suggestion.ToDictionary()
            );
    }
}
