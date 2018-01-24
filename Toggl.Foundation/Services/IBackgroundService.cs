using System;

namespace Toggl.Foundation.Services
{
    public interface IBackgroundService
    {
        void EnterBackground();
        void EnterForeground();

        IObservable<TimeSpan> AppResumedFromBackground { get; }
    }
}
