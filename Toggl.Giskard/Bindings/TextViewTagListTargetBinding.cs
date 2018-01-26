using System;
using System.Collections.Generic;
using System.Linq;
using Android.Widget;
using MvvmCross.Binding;
using MvvmCross.Binding.Bindings.Target;

namespace Toggl.Giskard.Bindings
{
    public sealed class TextViewTagListTargetBinding
        : MvxTargetBinding<TextView, IEnumerable<string>>
    {
        public const string BindingName = "Tags";

        public override MvxBindingMode DefaultMode => MvxBindingMode.OneWay;

        public TextViewTagListTargetBinding(TextView target) : base(target)
        {
        }

        protected override void SetValue(IEnumerable<string> value)
        {
            if (value == null || !value.Any())
                return;

            Target.Text = string.Join(" ", value);
        }
    }
}
