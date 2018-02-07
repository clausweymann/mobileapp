using System.Collections.Generic;
using System.Linq;
using Firebase.Analytics;
using Foundation;
using Toggl.Foundation.Analytics;

namespace Toggl.Daneel.Services
{
    public sealed class AnalyticsService : BaseAnalyticsService
    {
        protected override void NativeTrackEvent(string eventName, Dictionary<string, string> parameters)
        {
            Analytics.LogEvent(new NSString(eventName), NSDictionary<NSString, NSObject>.FromObjectsAndKeys(
                parameters.Values.ToArray(),
                parameters.Keys.ToArray()
            ));
        }
    }
}
