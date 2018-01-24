using System;
using Android.Views;

namespace Toggl.Giskard.Extensions
{
    public static class VisibilityExtensions
    {
        public static bool ToBool(this ViewStates state)
            => state == ViewStates.Visible;

        public static ViewStates ToVisibility(this bool state)
            => state ? ViewStates.Visible : ViewStates.Invisible;
    }
}
