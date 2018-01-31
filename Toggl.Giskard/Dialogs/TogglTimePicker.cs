using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Views;

namespace Toggl.Giskard.Dialogs
{
    public sealed class TogglTimePicker
    {
        private TaskCompletionSource<DateTimeOffset> completionSource;
        private Context context;
        private DateTimeOffset date;
        private bool is24hourView;

        public TogglTimePicker(Context context, DateTimeOffset? date, bool is24hourView = true)
        {
            this.context = context;
            this.date = date ?? DateTimeOffset.Now;
            this.is24hourView = is24hourView;
        }

        public Task<DateTimeOffset> Show(string title = "")
        {
            completionSource = new TaskCompletionSource<DateTimeOffset>();

            var dialog = new TimePickerDialog(context, onTimeSet,
                                              date.Hour, date.Minute, is24hourView);

            dialog.SetTitle(title);

            dialog.Show();

            return completionSource.Task;
        }

        private void onTimeSet(object sender, TimePickerDialog.TimeSetEventArgs e)
        {
            var selectedDate = new DateTimeOffset(
                date.Year, date.Month, date.Day,
                e.HourOfDay, e.Minute, date.Second, date.Offset);

            completionSource.SetResult(selectedDate);
        }
    }

    public static class TogglTimePickerExtensions
    {
        public static void BindTimePickerToClick(this View view, DateTimeOffset? initialValue, Action<DateTimeOffset> onTimeSelected)
        {
            view.Click += async (s, e) =>
                onTimeSelected(await new TogglTimePicker(view.Context, initialValue).Show());
        }
    }
}