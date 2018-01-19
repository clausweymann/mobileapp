﻿using Toggl.Foundation.Suggestions.Interfaces;
using Toggl.PrimeRadiant.Models;

namespace Toggl.Foundation.Suggestions
{
    public sealed class Suggestion : ISuggestion
    {
        public string Description { get; } = "";

        public long? ProjectId { get; } = null;

        public long? TaskId { get; } = null;

        public string ProjectColor { get; } = "";

        public string ProjectName { get; } = "";

        public string TaskName { get; } = "";

        public string ClientName { get; } = "";

        public bool HasProject { get; } = false;

        public long WorkspaceId { get; }

        internal Suggestion(IDatabaseTimeEntry timeEntry)
        {
            TaskId = timeEntry.TaskId;
            ProjectId = timeEntry.ProjectId;
            Description = timeEntry.Description;
            WorkspaceId = timeEntry.WorkspaceId;

            if (timeEntry.Project == null) return;

            HasProject = true;
            ProjectName = timeEntry.Project.Name;
            ProjectColor = timeEntry.Project.Color;

            ClientName = timeEntry.Project.Client?.Name ?? "";

            if (timeEntry.Task == null) return;

            TaskName = timeEntry.Task.Name;
        }
    }
}
