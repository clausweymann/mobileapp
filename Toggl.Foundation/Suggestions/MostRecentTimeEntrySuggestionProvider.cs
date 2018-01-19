using System;
using System.Linq;
using System.Reactive.Linq;
using Toggl.Foundation.DataSources;
using Toggl.Multivac;
using static Toggl.Multivac.Extensions.EnumerableExtensions;

namespace Toggl.Foundation.Suggestions
{
    public class MostRecentTimeEntrySuggestionProvider : ISuggestionProvider
    {
        private readonly ITogglDataSource dataSource;

        public MostRecentTimeEntrySuggestionProvider(ITogglDataSource dataSource)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));

            this.dataSource = dataSource;
        }

        public IObservable<Suggestion> GetSuggestions()
            => dataSource
                .TimeEntries
                .GetAll()
                .Select(timeEntries => timeEntries.MaxFor(te => te.Start))
                .Select(timeEntry => new Suggestion(timeEntry));
    }
}
