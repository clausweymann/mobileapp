﻿using Toggl.Multivac;
using Toggl.Multivac.Models;


namespace Toggl.Ultrawave.Models
{
    [Preserve(AllMembers = true)]
    internal sealed partial class Client
    {
        public Client() { }

        public Client(IClient entity)
        {
            Id = entity.Id;
            WorkspaceId = entity.WorkspaceId;
            Name = entity.Name;
            At = entity.At;
            ServerDeletedAt = entity.ServerDeletedAt;
        }
    }

    [Preserve(AllMembers = true)]
    internal sealed partial class Project
    {
        public Project() { }

        public Project(IProject entity)
        {
            Id = entity.Id;
            WorkspaceId = entity.WorkspaceId;
            ClientId = entity.ClientId;
            Name = entity.Name;
            IsPrivate = entity.IsPrivate;
            Active = entity.Active;
            At = entity.At;
            ServerDeletedAt = entity.ServerDeletedAt;
            Color = entity.Color;
            Billable = entity.Billable;
            Template = entity.Template;
            AutoEstimates = entity.AutoEstimates;
            EstimatedHours = entity.EstimatedHours;
            Rate = entity.Rate;
            Currency = entity.Currency;
            ActualHours = entity.ActualHours;
        }
    }

    [Preserve(AllMembers = true)]
    internal sealed partial class Tag
    {
        public Tag() { }

        public Tag(ITag entity)
        {
            Id = entity.Id;
            WorkspaceId = entity.WorkspaceId;
            Name = entity.Name;
            At = entity.At;
            DeletedAt = entity.DeletedAt;
        }
    }

    [Preserve(AllMembers = true)]
    internal sealed partial class Task
    {
        public Task() { }

        public Task(ITask entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            ProjectId = entity.ProjectId;
            WorkspaceId = entity.WorkspaceId;
            UserId = entity.UserId;
            EstimatedSeconds = entity.EstimatedSeconds;
            Active = entity.Active;
            At = entity.At;
            TrackedSeconds = entity.TrackedSeconds;
        }
    }

    [Preserve(AllMembers = true)]
    internal sealed partial class TimeEntry
    {
        public TimeEntry() { }

        public TimeEntry(ITimeEntry entity)
        {
            Id = entity.Id;
            WorkspaceId = entity.WorkspaceId;
            ProjectId = entity.ProjectId;
            TaskId = entity.TaskId;
            Billable = entity.Billable;
            Start = entity.Start;
            Duration = entity.Duration;
            Description = entity.Description;
            TagIds = entity.TagIds;
            At = entity.At;
            ServerDeletedAt = entity.ServerDeletedAt;
            UserId = entity.UserId;
        }
    }

    [Preserve(AllMembers = true)]
    internal sealed partial class User
    {
        public User() { }

        public User(IUser entity)
        {
            Id = entity.Id;
            ApiToken = entity.ApiToken;
            DefaultWorkspaceId = entity.DefaultWorkspaceId;
            Email = entity.Email;
            Fullname = entity.Fullname;
            TimeOfDayFormat = entity.TimeOfDayFormat;
            DateFormat = entity.DateFormat;
            BeginningOfWeek = entity.BeginningOfWeek;
            Language = entity.Language;
            ImageUrl = entity.ImageUrl;
            At = entity.At;
        }
    }

    [Preserve(AllMembers = true)]
    internal sealed partial class Workspace
    {
        public Workspace() { }

        public Workspace(IWorkspace entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            Admin = entity.Admin;
            SuspendedAt = entity.SuspendedAt;
            ServerDeletedAt = entity.ServerDeletedAt;
            DefaultHourlyRate = entity.DefaultHourlyRate;
            DefaultCurrency = entity.DefaultCurrency;
            OnlyAdminsMayCreateProjects = entity.OnlyAdminsMayCreateProjects;
            OnlyAdminsSeeBillableRates = entity.OnlyAdminsSeeBillableRates;
            OnlyAdminsSeeTeamDashboard = entity.OnlyAdminsSeeTeamDashboard;
            ProjectsBillableByDefault = entity.ProjectsBillableByDefault;
            Rounding = entity.Rounding;
            RoundingMinutes = entity.RoundingMinutes;
            At = entity.At;
            LogoUrl = entity.LogoUrl;
        }
    }

    [Preserve(AllMembers = true)]
    internal sealed partial class WorkspaceFeature
    {
        public WorkspaceFeature() { }

        public WorkspaceFeature(IWorkspaceFeature entity)
        {
            FeatureId = entity.FeatureId;
            Enabled = entity.Enabled;
        }
    }

    [Preserve(AllMembers = true)]
    internal sealed partial class WorkspaceFeatureCollection
    {
        public WorkspaceFeatureCollection() { }

        public WorkspaceFeatureCollection(IWorkspaceFeatureCollection entity)
        {
            WorkspaceId = entity.WorkspaceId;
            Features = entity.Features;
        }
    }
}