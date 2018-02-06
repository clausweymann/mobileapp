using System;
using System.Collections.Generic;

namespace Toggl.Foundation.Services
{
    public abstract class BaseAnalyticsService : IAnalyticsService
    {
        private const string pageParameter = "PageWhenSkipWasClicked";
        private const string onboardingSkipEventName = "OnboardingSkip";

        public void TrackOnboardingSkipEvent(string pageName)
        {
            var dict = new Dictionary<string, string> { { onboardingSkipEventName, pageName } };
            NativeTrackEvent(onboardingSkipEventName, dict);
        }

        public void TrackCurrentPage(Type viewModelType)
        {
        }

        public void TrackLoginEvent()
        {
        }

        public void TrackNonFatalException(Exception ex)
        {
        }

        public void TrackSignUpEvent()
        {
        }

        protected abstract void NativeTrackEvent(string eventName, Dictionary<string, string> parameters);
    }
}
