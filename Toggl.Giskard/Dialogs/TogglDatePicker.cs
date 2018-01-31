using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using MvvmCross.Platform;
using MvvmCross.Platform.Droid.Platform;

namespace Toggl.Giskard.Dialogs
{
    public sealed class TogglDatePicker
    {
        private TaskCompletionSource<DateTimeOffset> completionSource;
        private Context context;
        private DateTimeOffset date;

        public TogglDatePicker(Context context, DateTimeOffset? date)
        {
            this.context = context;
            this.date = date?? DateTimeOffset.Now;
        }

        public Task<DateTimeOffset> Show(string title = "")
        {
            completionSource = new TaskCompletionSource<DateTimeOffset>();

            var dialog = new DatePickerDialog(
                context, onDateSet, 
                date.Year, date.Month, date.Day);

            dialog.SetTitle(title);

            dialog.Show();

            return completionSource.Task;
        }

        private void onDateSet(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            var selectedDate = new DateTimeOffset(
                e.Year, e.Month + 1, e.DayOfMonth,
                date.Hour, date.Minute, date.Second, date.Offset);

            completionSource.SetResult(selectedDate);
        }
    }

    public static class TogglDatePickerExtensions
    {
        public static void BindDatePickerToClick(this View view, DateTimeOffset? initialValue, Action<DateTimeOffset> onDateSelected)
        {
            view.Click += async (s, e) =>
                onDateSelected(await new TogglDatePicker(view.Context, initialValue).Show());
        }
    }
}