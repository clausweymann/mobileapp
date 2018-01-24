using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Toggl.Multivac;

namespace Toggl.Foundation.Services
{
    public sealed class BackgroundService : IBackgroundService
    {
        private readonly ITimeService timeService;

        private DateTimeOffset? lastEnteredBackground { get; set; }
        private ISubject<TimeSpan> appBecameActiveSubject { get; }

        public IObservable<TimeSpan> AppResumedFromBackground { get; }

        public BackgroundService(ITimeService timeService)
        {
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));

            this.timeService = timeService;

            appBecameActiveSubject = new Subject<TimeSpan>();
            lastEnteredBackground = null;

            AppResumedFromBackground = appBecameActiveSubject.AsObservable();
        }

        public void EnterBackground()
            => lastEnteredBackground = timeService.CurrentDateTime;

        public void EnterForeground()
        {
            if (lastEnteredBackground.HasValue == false)
                return;

            var timeInBackground = timeService.CurrentDateTime - lastEnteredBackground.Value;
            lastEnteredBackground = null;
            appBecameActiveSubject.OnNext(timeInBackground);
        }
    }
}
