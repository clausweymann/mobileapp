using System;
using Android.App;
using Android.OS;
using Android.Support.V7.Widget;
using MvvmCross.Droid.Support.V7.AppCompat;
using MvvmCross.Droid.Views.Attributes;
using Toggl.Foundation.MvvmCross.ViewModels;
using Toggl.Giskard.Fragments;
using Toggl.Giskard.Dialogs;
using static Android.Support.V7.Widget.Toolbar;
using TextView = Android.Widget.TextView;

namespace Toggl.Giskard.Activities
{
    [MvxActivityPresentation]
    [Activity(Theme = "@style/AppTheme")]
    public class EditTimeEntryActivity : MvxAppCompatActivity<EditTimeEntryViewModel>
    {
        private IDisposable startDateDisposable;
        private IDisposable startTimeDisposable;
        private IDisposable stopTimeDisposable;
        
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.EditLayout);

            OverridePendingTransition(Resource.Animation.abc_fade_in, Resource.Animation.abc_fade_out);

            setupToolbar();

            startDateDisposable = FindViewById<TextView>(Resource.Id.EditDateValueLabel)
                .BindDatePickerToClick(
                    () => ViewModel.StartTime, 
                    dateTime => ViewModel.StartTime = dateTime);
            
            startTimeDisposable = FindViewById<TextView>(Resource.Id.EditStartValueLabel)
                .BindTimePickerToClick(
                    () => ViewModel.StartTime, 
                    dateTime => ViewModel.StartTime = dateTime);

            stopTimeDisposable = FindViewById<TextView>(Resource.Id.EditEndValueLabel)
                .BindTimePickerToClick(
                    () => ViewModel.StopTime, 
                    dateTime => ViewModel.StopTime = dateTime);
        }

        private void setupToolbar()
        {
            var toolbar = FindViewById<Toolbar>(Resource.Id.Toolbar);

            toolbar.Title = Resources.GetString(Resource.String.Edit);

            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);

            toolbar.NavigationClick += onNavigateBack;
        }

        private void onNavigateBack(object sender, NavigationClickEventArgs e)
        {
            ViewModel.CloseCommand.Execute();
        }

        public override void Finish()
        {
            base.Finish();
        }

        protected override void Dispose(bool disposing)
        {
            startDateDisposable.Dispose();
            startTimeDisposable.Dispose();
            stopTimeDisposable.Dispose();

            base.Dispose(disposing);
        }
    }


}
