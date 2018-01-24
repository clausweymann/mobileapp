using Android.Content;
using Android.Util;

namespace Toggl.Giskard.Extensions
{
    public static class NumberExtensions
    {
        public static float DpToPixels(this int self, Context context)
            => ((float)self).DpToPixels(context);

        public static float DpToPixels(this float self, Context context)
            => TypedValue.ApplyDimension(ComplexUnitType.Dip, self, context.Resources.DisplayMetrics);
    }
}
