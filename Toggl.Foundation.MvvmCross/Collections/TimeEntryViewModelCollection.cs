using System;
using System.Collections.Generic;
using System.Linq;
using MvvmCross.Core.ViewModels;
using Toggl.Foundation.MvvmCross.ViewModels;
using Toggl.Multivac;

namespace Toggl.Foundation.MvvmCross.Collections
{
    [Preserve(AllMembers = true)]
    public sealed class TimeEntryViewModelCollection : MvxObservableCollection<TimeEntryViewModel>
    {
        public DateTimeOffset Date { get; }

        public TimeSpan TotalTime { get; }

        public TimeEntryViewModelCollection(DateTime date, IEnumerable<TimeEntryViewModel> items)
        {
            Ensure.Argument.IsNotNull(items, nameof(items));

            if (date.Kind != DateTimeKind.Local)
                throw new ArgumentException($"{nameof(date)} must have kind DateTimeKind.Local");

            this.AddRange(items);

            Date = new DateTimeOffset(date);
            TotalTime = items.Aggregate(TimeSpan.Zero, (acc, vm) => acc + vm.Duration);
        }
    }
}
