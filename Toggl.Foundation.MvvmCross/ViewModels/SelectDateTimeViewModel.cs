using System;
using System.Threading.Tasks;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using Toggl.Foundation.MvvmCross.Parameters;
using Toggl.Multivac;
using Toggl.Multivac.Extensions;
using PickerMode = Toggl.Foundation.MvvmCross.Parameters.DatePickerParameters.PickerMode;

namespace Toggl.Foundation.MvvmCross.ViewModels
{
    [Preserve(AllMembers = true)]
    public class SelectDateTimeViewModel : MvxViewModel<DatePickerParameters, DateTimeOffset>
    {
        private DateTimeOffset defaultResult;

        private readonly IMvxNavigationService navigationService;

        private DateTimeOffset currentDateTime;
        public DateTimeOffset CurrentDateTime
        {
            get => currentDateTime;
            set
            {
                var newValue = convert(value);
                if (currentDateTime == newValue) return;

                currentDateTime = newValue;

                RaisePropertyChanged(nameof(CurrentDateTime));
            }
        }

        public DateTimeOffset MinDate { get; private set; }

        public DateTimeOffset MaxDate { get; private set; }
       
        public PickerMode Mode { get; private set; }

        public IMvxAsyncCommand CloseCommand { get; }

        public IMvxAsyncCommand SaveCommand { get; set; }

        public SelectDateTimeViewModel(IMvxNavigationService navigationService)
        {
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));

            this.navigationService = navigationService;

            CloseCommand = new MvxAsyncCommand(close);
            SaveCommand = new MvxAsyncCommand(save);
        }

        private void OnCurrentDateTimeChanged()
        {
            CurrentDateTime = CurrentDateTime.Clamp(MinDate, MaxDate);
        }

        public override void Prepare(DatePickerParameters parameter)
        {
            Mode = parameter.Mode;
            MinDate = parameter.MinDate;
            MaxDate = parameter.MaxDate;
            CurrentDateTime = defaultResult = parameter.CurrentDate;
        }

        private DateTimeOffset convert(DateTimeOffset offset)
        {
            switch (Mode)
            {
                case PickerMode.Date:
                    return new DateTimeOffset(offset.Year, offset.Month, offset.Day,
                                              currentDateTime.Hour, currentDateTime.Minute, currentDateTime.Second, 
                                              currentDateTime.Offset);
                case PickerMode.Time:
                    return new DateTimeOffset(currentDateTime.Year, currentDateTime.Month, currentDateTime.Day,
                                              offset.Hour, offset.Minute, offset.Second, 
                                              currentDateTime.Offset);
                case PickerMode.DateTime:
                    return offset;
                default:
                    throw new NotSupportedException("Invalid DateTimePicker mode");
            }
        }

        private Task close() => navigationService.Close(this, defaultResult);

        private Task save() => navigationService.Close(this, CurrentDateTime);
    }
}
