using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Foundation;
using Toggl.Foundation.DataSources;
using Toggl.Multivac;
using UIKit;

namespace Toggl.Daneel.Services.ApplicationShortcuts
{
    public class StopTimeEntryApplicationShortcutHandler : IApplicationShortcutHandler
    {
        private readonly ITogglDataSource dataSource;
        private readonly ITimeService timeService;

        public StopTimeEntryApplicationShortcutHandler(
            ITogglDataSource dataSource, ITimeService timeService)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));

            this.dataSource = dataSource;
            this.timeService = timeService;
        }

        public async Task Handle(UIApplicationShortcutItem shortcut)
            => await dataSource.TimeEntries.Stop(timeService.CurrentDateTime);
    }
}
