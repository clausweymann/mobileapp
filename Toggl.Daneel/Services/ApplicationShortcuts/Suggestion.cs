using System;
using Foundation;
using Toggl.Foundation.Suggestions.Interfaces;

namespace Toggl.Daneel.Services.ApplicationShortcuts
{
    public class Suggestion : ISuggestion
    {
        public long? TaskId { get; }

        public string TaskName { get; }

        public bool HasProject { get; }

        public long? ProjectId { get; }

        public long WorkspaceId { get; }

        public string ClientName { get; }

        public string Description { get; }

        public string ProjectName { get; }

        public string ProjectColor { get; }

        private Suggestion(NSDictionary<NSString, NSObject> dictionary)
        {
            TaskId = nullableLongFromNsObject(dictionary[nameof(TaskId)]);
            TaskName = stringFromNsObject(dictionary[nameof(TaskName)]);
            ProjectId = nullableLongFromNsObject(dictionary[nameof(ProjectId)]);
            HasProject = boolFromNsObject(dictionary[nameof(HasProject)]);
            ClientName = stringFromNsObject(dictionary[nameof(ClientName)]);
            ProjectName = stringFromNsObject(dictionary[nameof(ProjectName)]);
            Description = stringFromNsObject(dictionary[nameof(Description)]);
            WorkspaceId = longFromNsObject(dictionary[nameof(WorkspaceId)]);
            ProjectColor = stringFromNsObject(dictionary[nameof(ProjectColor)]);
        }

        public Suggestion(ISuggestion suggestion)
        {
            TaskId = suggestion.TaskId;
            TaskName = suggestion.TaskName;
            ProjectId = suggestion.ProjectId;
            HasProject = suggestion.HasProject;
            ClientName = suggestion.ClientName;
            ProjectName = suggestion.ProjectName;
            Description = suggestion.Description;
            WorkspaceId = suggestion.WorkspaceId;
            ProjectColor = suggestion.ProjectColor;
        }

        public NSDictionary<NSString, NSObject> ToDictionary()
        {
            return new NSDictionary<NSString, NSObject>(
                new NSString[]
                {
                    new NSString(nameof(TaskId)),
                    new NSString(nameof(TaskName)),
                    new NSString(nameof(ProjectId)),
                    new NSString(nameof(HasProject)),
                    new NSString(nameof(ClientName)),
                    new NSString(nameof(ProjectName)),
                    new NSString(nameof(Description)),
                    new NSString(nameof(WorkspaceId)),
                    new NSString(nameof(ProjectColor))
                },
                new NSObject[]
                {
                    toNsNumber(TaskId),
                    new NSString(TaskName),
                    toNsNumber(ProjectId),
                    new NSNumber(HasProject),
                    new NSString(ClientName),
                    new NSString(ProjectName),
                    new NSString(Description),
                    toNsNumber(WorkspaceId),
                    new NSString(ProjectColor)
                }
            );
        }

        public static ISuggestion FromDictionary(NSDictionary<NSString, NSObject> dictionary)
            => new Suggestion(dictionary);

        private static NSObject toNsNumber(long? number)
        {
            if (number.HasValue)
                return new NSNumber(number.Value);

            return new NSString("");
        }

        private static long? nullableLongFromNsObject(NSObject obj)
        {
            if (obj is NSString)
                return null;

            if (obj is NSNumber number)
                return number.LongValue;

            throw new ArgumentException($"{nameof(obj)} should have been of type NSNull or NSNumber, but it actually was {obj.GetType()}");
        }

        private static long longFromNsObject(NSObject obj)
        {
            if (obj is NSNumber number)
                return number.LongValue;

            throw new ArgumentException($"{nameof(obj)} should have been of type NSNumber, but it actually was {obj.GetType()}");
        }

        private static string stringFromNsObject(NSObject obj)
        {
            if (obj is NSString str)
                return str;

            throw new ArgumentException($"{nameof(obj)} should have been of type NSString, but actually it was {obj.GetType()}");
        }

        private static bool boolFromNsObject(NSObject obj)
        {
            if (obj is NSNumber number)
                return number.BoolValue;

            throw new ArgumentException($"{nameof(obj)} should have been of type NSNumber, but actually it was {obj.GetType()}");
        }
    }
}
