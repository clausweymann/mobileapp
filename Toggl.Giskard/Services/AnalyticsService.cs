using System.Collections.Generic;
using Toggl.Foundation.Services;

namespace Toggl.Giskard.Services
{
    public sealed class AnalyticsService : BaseAnalyticsService
    {
        protected override void NativeTrackEvent(string eventName, Dictionary<string, string> parameters)
        {
        }
    }
}
