using System;
using Android.Graphics.Drawables;
using Android.Views;
using MvvmCross.Binding;
using MvvmCross.Binding.Droid.Target;
using Toggl.Giskard.Extensions;
using MvvmCross.Plugins.Color.Droid;
using MvvmCross.Platform.UI;

namespace Toggl.Giskard.Bindings
{
    public sealed class DrawableColorTargetBinding : MvxAndroidTargetBinding<View, string>
    {
        public const string BindingName = "DrawableColor";

        public DrawableColorTargetBinding(View target) : base(target)
        {
        }

        public override MvxBindingMode DefaultMode => MvxBindingMode.Default;

        protected override void SetValueImpl(View target, string value)
        {
            if (target?.Background is GradientDrawable drawable)
            {
                drawable.SetColor(MvxColorExtensions.ToAndroidColor(MvxColor.ParseHexString(value)));
                drawable.InvalidateSelf();
            }
        }
    }
}
