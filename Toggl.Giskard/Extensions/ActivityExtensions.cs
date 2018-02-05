using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Views;

namespace Toggl.Giskard.Extensions
{
    public static class ActivityExtensions
    {
        public static void ChangeStatusBarColor(this Activity activity, Color color, bool useDarkIcons = false)
        {
            var window = activity.Window;
            window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
            window.ClearFlags(WindowManagerFlags.TranslucentStatus);
            window.SetStatusBarColor(color);

            if (Build.VERSION.SdkInt < BuildVersionCodes.M) return;

            window.DecorView.SystemUiVisibility =
                (StatusBarVisibility)(useDarkIcons ? SystemUiFlags.LightStatusBar : SystemUiFlags.Visible);
        }
    }
}
