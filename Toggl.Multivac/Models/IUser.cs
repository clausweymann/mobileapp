using System;

namespace Toggl.Multivac.Models
{
    public interface IUser : IBaseModel
    {
        string ApiToken { get; }

        long DefaultWorkspaceId { get; }

        Email Email { get; }

        string Fullname { get; }

        string TimeOfDayFormat { get; }

        string DateFormat { get; }

        BeginningOfWeek BeginningOfWeek { get; }

        string Language { get; }

        string ImageUrl { get; }

        DateTimeOffset At { get; }
    }
}
