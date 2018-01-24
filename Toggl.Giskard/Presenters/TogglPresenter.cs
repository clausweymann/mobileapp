using System;
using System.Collections.Generic;
using System.Reflection;
using MvvmCross.Core.ViewModels;
using MvvmCross.Droid.Support.V7.AppCompat;
using Toggl.Foundation.MvvmCross.ViewModels.Hints;
using Toggl.Giskard.Activities;

namespace Toggl.Giskard.Presenters
{
    public sealed class TogglPresenter : MvxAppCompatViewPresenter
    {
        public TogglPresenter(IEnumerable<Assembly> androidViewAssemblies) 
            : base(androidViewAssemblies)
        {
        }

        public override void ChangePresentation(MvxPresentationHint hint)
        {
            switch (hint)
            {
                case CardVisibilityHint cardHint:
                    if(CurrentActivity is MainActivity mainActivity)
                        mainActivity.OnTimeEntryCardVisibilityChanged(cardHint.Visible);
                    
                    return;

                case ToggleCalendarVisibilityHint calendarHint:
                    return;
            }

            base.ChangePresentation(hint);
        }
       
    }
}
