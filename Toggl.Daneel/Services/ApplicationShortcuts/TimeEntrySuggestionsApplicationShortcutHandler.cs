using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Foundation;
using Toggl.Foundation.DataSources;
using Toggl.Foundation.DTOs;
using Toggl.Multivac;
using UIKit;

namespace Toggl.Daneel.Services.ApplicationShortcuts
{
    public class TimeEntrySuggestionsApplicationShortcutHandler
        : IApplicationShortcutHandler
    {
        private readonly ITogglDataSource dataSource;
        private readonly ITimeService timeService;

        public TimeEntrySuggestionsApplicationShortcutHandler(
            ITogglDataSource dataSource, ITimeService timeService)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));

            this.dataSource = dataSource;
            this.timeService = timeService;
        }

        public async Task Handle(UIApplicationShortcutItem shortcut)
        {
            var suggestion = Suggestion.FromDictionary(shortcut.UserInfo);

            await dataSource.User
                .Current
                .Select(user => new StartTimeEntryDTO
                {
                    UserId = user.Id,
                    Billable = true, //TODO
                    ProjectId = suggestion.ProjectId,
                    TaskId = suggestion.TaskId,
                    Description = suggestion.Description,
                    WorkspaceId = suggestion.WorkspaceId,
                    StartTime = timeService.CurrentDateTime
                })
                .SelectMany(dataSource.TimeEntries.Start)
                .Do(_ => dataSource.SyncManager.PushSync());

            //var user = await dataSource.User.Current;

            //var startTimeEntryDto = new StartTimeEntryDTO
            //{
            //    UserId = user.Id,
            //    Billable = true, //TODO
            //    ProjectId = suggestion.ProjectId,
            //    TaskId = suggestion.TaskId,
            //    Description = suggestion.Description,
            //    WorkspaceId = suggestion.WorkspaceId,
            //    StartTime = timeService.CurrentDateTime
            //};

            //await dataSource.TimeEntries.Start(startTimeEntryDto);
        }
    }
}
