namespace Toggl.Foundation.Suggestions.Interfaces
{
    public interface ISuggestion
    {
        string Description { get; }

        long? ProjectId { get; }

        long? TaskId { get; }

        string ProjectColor { get; }

        string ProjectName { get; }

        string TaskName { get; }

        string ClientName { get; }

        bool HasProject { get; }

        long WorkspaceId { get; }
    }
}
