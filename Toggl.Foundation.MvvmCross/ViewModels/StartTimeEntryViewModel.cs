﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using Toggl.Foundation.Autocomplete;
using Toggl.Foundation.Autocomplete.Suggestions;
using Toggl.Foundation.DataSources;
using Toggl.Foundation.DTOs;
using Toggl.Foundation.MvvmCross.Collections;
using Toggl.Foundation.MvvmCross.Helper;
using Toggl.Foundation.MvvmCross.Parameters;
using Toggl.Foundation.MvvmCross.Services;
using Toggl.Multivac;
using Toggl.Multivac.Extensions;
using static Toggl.Foundation.Helper.Constants;

namespace Toggl.Foundation.MvvmCross.ViewModels
{
    [Preserve(AllMembers = true)]
    public sealed class StartTimeEntryViewModel : MvxViewModel<StartTimeEntryParameters>
    {
        //Fields
        private readonly ITimeService timeService;
        private readonly IDialogService dialogService;
        private readonly ITogglDataSource dataSource;
        private readonly IMvxNavigationService navigationService;
        private readonly Subject<TextFieldInfo> infoSubject = new Subject<TextFieldInfo>();
        private readonly Subject<AutocompleteSuggestionType> queryByTypeSubject = new Subject<AutocompleteSuggestionType>();

        private bool hasAnyTags;
        private bool hasAnyProjects;
        private long? lastProjectId;
        private IDisposable queryDisposable;
        private IDisposable elapsedTimeDisposable;
        private TextFieldInfo previousTextFieldInfo;
        private StartTimeEntryParameters parameter;

        private TimeSpan displayedTime = TimeSpan.Zero;

        //Properties
        private int DescriptionByteCount
            => TextFieldInfo.Text.LengthInBytes();

        public int DescriptionRemainingBytes
            => MaxTimeEntryDescriptionLengthInBytes - DescriptionByteCount;

        public bool DescriptionLengthExceeded
            => DescriptionByteCount > MaxTimeEntryDescriptionLengthInBytes;

        public bool SuggestCreation
        {
            get
            {
                if (IsSuggestingProjects && TextFieldInfo.ProjectId.HasValue) return false;

                if (string.IsNullOrEmpty(CurrentQuery))
                    return false;

                if (IsSuggestingProjects)
                    return !Suggestions.Any(c => c.Any(s => s is ProjectSuggestion pS && pS.ProjectName == CurrentQuery))
                           && CurrentQuery.LengthInBytes() <= MaxProjectNameLengthInBytes;

                if (IsSuggestingTags)
                    return !Suggestions.Any(c => c.Any(s => s is TagSuggestion tS && tS.Name == CurrentQuery))
                           && CurrentQuery.LengthInBytes() <= MaxTagNameLengthInBytes;

                return false;
            }
        }

        public bool IsDirty
            => TextFieldInfo.Text.Length > 0
                || TextFieldInfo.ProjectId.HasValue
                || TextFieldInfo.Tags.Length > 0
                || IsBillable
                || StartTime != parameter.StartTime
                || Duration != parameter.Duration;

        public bool UseGrouping { get; set; }

        public string CurrentQuery { get; private set; }

        public bool IsEditingTime { get; private set; }

        public bool IsSuggestingTags { get; private set; }

        public bool IsSuggestingProjects { get; private set; }

        public TextFieldInfo TextFieldInfo { get; set; } = TextFieldInfo.Empty;

        public TimeSpan DisplayedTime
        {
            get => displayedTime;
            set
            {
                if (elapsedTimeDisposable != null)
                {
                    StartTime = timeService.CurrentDateTime - value;
                }
                else
                {
                    Duration = value;
                }

                displayedTime = value; 
            }
        }

        public bool IsBillable { get; private set; } = false;

        public bool IsBillableAvailable { get; private set; } = false;

        public bool IsEditingProjects { get; private set; } = false;

        public bool IsEditingTags { get; private set; } = false;

        public string PlaceholderText { get; private set; }

        public bool ShouldShowNoTagsInfoMessage
            => IsSuggestingTags && !hasAnyTags;

        public bool ShouldShowNoProjectsInfoMessage
            => IsSuggestingProjects && !hasAnyProjects;

        public DateTimeOffset StartTime { get; private set; }

        public TimeSpan? Duration { get; private set; }

        public MvxObservableCollection<WorkspaceGroupedCollection<AutocompleteSuggestion>> Suggestions { get; }
            = new MvxObservableCollection<WorkspaceGroupedCollection<AutocompleteSuggestion>>();

        public IMvxAsyncCommand BackCommand { get; }

        public IMvxAsyncCommand DoneCommand { get; }

        public IMvxAsyncCommand SetStartDateCommand { get; }

        public IMvxAsyncCommand ChangeTimeCommand { get; }

        public IMvxCommand ToggleBillableCommand { get; }

        public IMvxAsyncCommand CreateCommand { get; }
        
        public IMvxCommand ToggleTagSuggestionsCommand { get; }

        public IMvxCommand ToggleProjectSuggestionsCommand { get; }

        public IMvxAsyncCommand<AutocompleteSuggestion> SelectSuggestionCommand { get; }

        public IMvxCommand<ProjectSuggestion> ToggleTaskSuggestionsCommand { get; }

        public StartTimeEntryViewModel(
            ITimeService timeService,
            IDialogService dialogService,
            ITogglDataSource dataSource,
            IMvxNavigationService navigationService)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));
            Ensure.Argument.IsNotNull(dialogService, nameof(dialogService));
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));

            this.dataSource = dataSource;
            this.timeService = timeService;
            this.dialogService = dialogService;
            this.navigationService = navigationService;

            BackCommand = new MvxAsyncCommand(back);
            DoneCommand = new MvxAsyncCommand(done);
            ToggleBillableCommand = new MvxCommand(toggleBillable);
            CreateCommand = new MvxAsyncCommand(create);
            ChangeTimeCommand = new MvxAsyncCommand(changeTime);
            SetStartDateCommand = new MvxAsyncCommand(setStartDate);
            ToggleTagSuggestionsCommand = new MvxCommand(toggleTagSuggestions);
            ToggleProjectSuggestionsCommand = new MvxCommand(toggleProjectSuggestions);
            SelectSuggestionCommand = new MvxAsyncCommand<AutocompleteSuggestion>(selectSuggestion);
            ToggleTaskSuggestionsCommand = new MvxCommand<ProjectSuggestion>(toggleTaskSuggestions);
        }

        private async Task selectSuggestion(AutocompleteSuggestion suggestion)
        {
            switch (suggestion)
            {
                case QuerySymbolSuggestion querySymbolSuggestion:
                    TextFieldInfo = TextFieldInfo.WithTextAndCursor(querySymbolSuggestion.Symbol, 1);
                    break;

                case TimeEntrySuggestion timeEntrySuggestion:

                    TextFieldInfo = TextFieldInfo.WithTextAndCursor(
                        timeEntrySuggestion.Description,
                        timeEntrySuggestion.Description.Length);

                    if (!timeEntrySuggestion.ProjectId.HasValue)
                    {
                        TextFieldInfo = TextFieldInfo.RemoveProjectInfo();
                        return;
                    }

                    if (timeEntrySuggestion.TaskId == null)
                    {
                        TextFieldInfo = TextFieldInfo.WithProjectInfo(
                            timeEntrySuggestion.WorkspaceId,
                            timeEntrySuggestion.ProjectId.Value,
                            timeEntrySuggestion.ProjectName,
                            timeEntrySuggestion.ProjectColor);
                        break;
                    }

                    TextFieldInfo = TextFieldInfo.WithProjectAndTaskInfo(
                        timeEntrySuggestion.WorkspaceId,
                        timeEntrySuggestion.ProjectId.Value,
                        timeEntrySuggestion.ProjectName,
                        timeEntrySuggestion.ProjectColor,
                        timeEntrySuggestion.TaskId.Value,
                        timeEntrySuggestion.TaskName);
                    break;

                case ProjectSuggestion projectSuggestion:

                    if (TextFieldInfo.WorkspaceId == projectSuggestion.WorkspaceId)
                    {
                        setProject(projectSuggestion);
                        break;
                    }

                    var shouldChangeProject = await dialogService.Confirm(
                        Resources.DifferentWorkspaceAlertTitle,
                        Resources.DifferentWorkspaceAlertMessage,
                        Resources.Ok,
                        Resources.Cancel);

                    if (!shouldChangeProject) break;

                    setProject(projectSuggestion);

                    break;

                case TaskSuggestion taskSuggestion:

                    if (TextFieldInfo.WorkspaceId == taskSuggestion.WorkspaceId)
                    {
                        setTask(taskSuggestion);
                        break;
                    }

                    var shouldChangeTask = await dialogService.Confirm(
                        Resources.DifferentWorkspaceAlertTitle,
                        Resources.DifferentWorkspaceAlertMessage,
                        Resources.Ok,
                        Resources.Cancel);

                    if (!shouldChangeTask) break;

                    setTask(taskSuggestion);

                    break;

                case TagSuggestion tagSuggestion:

                    TextFieldInfo = TextFieldInfo
                        .RemoveTagQueryFromDescriptionIfNeeded()
                        .AddTag(tagSuggestion);
                    break;
            }
        }

        private async Task create()
        {
            if (IsSuggestingProjects)
                await createProject();
            else
                await createTag();
        }

        private async Task createProject()
        {
            var projectId = await navigationService.Navigate<EditProjectViewModel, string, long?>(CurrentQuery);
            if (projectId == null) return;

            var project = await dataSource.Projects.GetById(projectId.Value);
            var projectSuggestion = new ProjectSuggestion(project);
            
            setProject(projectSuggestion);
            hasAnyProjects = true;
        }

        private async Task createTag()
        {
            var createdTag = await dataSource.Tags
                .Create(CurrentQuery, TextFieldInfo.WorkspaceId.Value);
            var tagSuggestion = new TagSuggestion(createdTag);
            await SelectSuggestionCommand.ExecuteAsync(tagSuggestion);
            hasAnyTags = true;
        }

        private void setProject(ProjectSuggestion projectSuggestion)
        {
            if (TextFieldInfo.WorkspaceId.HasValue)
                clearTagsIfNeeded(TextFieldInfo.WorkspaceId.Value, projectSuggestion.WorkspaceId);

            TextFieldInfo = TextFieldInfo
                .RemoveProjectQueryFromDescriptionIfNeeded()
                .WithProjectInfo(
                    projectSuggestion.WorkspaceId,
                    projectSuggestion.ProjectId,
                    projectSuggestion.ProjectName,
                    projectSuggestion.ProjectColor);
        }

        private void setTask(TaskSuggestion taskSuggestion)
        {
            if (TextFieldInfo.WorkspaceId.HasValue)
                clearTagsIfNeeded(TextFieldInfo.WorkspaceId.Value, taskSuggestion.WorkspaceId);

            TextFieldInfo = TextFieldInfo
                .RemoveProjectQueryFromDescriptionIfNeeded()
                .WithProjectAndTaskInfo(
                    taskSuggestion.WorkspaceId,
                    taskSuggestion.ProjectId,
                    taskSuggestion.ProjectName,
                    taskSuggestion.ProjectColor,
                    taskSuggestion.TaskId,
                    taskSuggestion.Name
                );
        }

        private void clearTagsIfNeeded(long currentWorkspaceId, long newWorkspaceId)
        {
            if (currentWorkspaceId == newWorkspaceId) return;

            TextFieldInfo = TextFieldInfo.ClearTags();
        }

        public override void Prepare(StartTimeEntryParameters parameter)
        {
            this.parameter = parameter;
            StartTime = parameter.StartTime;
            Duration = parameter.Duration;

            if (Duration.HasValue)
            {
                displayedTime = Duration.Value;
                RaisePropertyChanged(nameof(DisplayedTime));
            }
            else
            {
                elapsedTimeDisposable = timeService.CurrentDateTimeObservable.Subscribe(onCurrentTime);
            }

            var queryByTypeObservable =
                queryByTypeSubject
                    .AsObservable()
                    .SelectMany(type => dataSource.AutocompleteProvider.Query(new QueryInfo("", type)));

            queryDisposable =
                infoSubject.AsObservable()
                    .StartWith(TextFieldInfo)
                    .Where(shouldUpdateSuggestions)
                    .Select(QueryInfo.ParseFieldInfo)
                    .Do(onParsedQuery)
                    .SelectMany(dataSource.AutocompleteProvider.Query)
                    .Merge(queryByTypeObservable)
                    .Subscribe(onSuggestions);

            PlaceholderText = parameter.PlaceholderText;
        }

        public async override Task Initialize()
        {
            await base.Initialize();

            await setBillableValues(0);

            var workspaceId = (await dataSource.User.Current).DefaultWorkspaceId;
            if (!TextFieldInfo.WorkspaceId.HasValue)
                TextFieldInfo = TextFieldInfo.WithWorkspace(workspaceId);
            
            hasAnyTags = (await dataSource.Tags.GetAll()).Any();
            hasAnyProjects = (await dataSource.Projects.GetAll()).Any();
        }

        private void onCurrentTime(DateTimeOffset currentTime)
        {
            displayedTime = currentTime - StartTime;
            RaisePropertyChanged(nameof(DisplayedTime));
        }

        private async void OnTextFieldInfoChanged()
        {
            infoSubject.OnNext(TextFieldInfo);

            if (TextFieldInfo.ProjectId == lastProjectId) return;
            lastProjectId = TextFieldInfo.ProjectId;
            await setBillableValues(lastProjectId ?? 0);
        }

        private void toggleTagSuggestions()
        {
            if (IsSuggestingTags)
            {
                TextFieldInfo = TextFieldInfo.RemoveTagQueryFromDescriptionIfNeeded();
                IsSuggestingTags = false;
                return;
            }

            appendSymbol(QuerySymbols.TagsString);
        }

        private void toggleProjectSuggestions()
        {
            if (IsSuggestingProjects)
            {
                TextFieldInfo = TextFieldInfo.RemoveProjectQueryFromDescriptionIfNeeded();
                IsSuggestingProjects = false;
                return;
            }

            if (TextFieldInfo.ProjectId != null)
            {
                queryByTypeSubject.OnNext(AutocompleteSuggestionType.Projects);
                IsSuggestingProjects = true;
                return;
            }

            appendSymbol(QuerySymbols.ProjectsString);
        }

        private void appendSymbol(string symbol)
        {
            var shouldAddWhitespace = TextFieldInfo.CursorPosition > 0 && Char.IsWhiteSpace(TextFieldInfo.Text[TextFieldInfo.CursorPosition - 1]) == false;
            var textToInsert = shouldAddWhitespace ? $" {symbol}" : symbol;
            var newText = TextFieldInfo.Text.Insert(TextFieldInfo.CursorPosition, textToInsert);
            TextFieldInfo = TextFieldInfo.WithTextAndCursor(newText, TextFieldInfo.CursorPosition + textToInsert.Length);            
        }

        private void toggleTaskSuggestions(ProjectSuggestion projectSuggestion)
        {
            var grouping = Suggestions.FirstOrDefault(s => s.WorkspaceName == projectSuggestion.WorkspaceName);
            if (grouping == null) return;

            var suggestionIndex = grouping.IndexOf(projectSuggestion);
            if (suggestionIndex < 0) return;

            projectSuggestion.TasksVisible = !projectSuggestion.TasksVisible;

            var groupingIndex = Suggestions.IndexOf(grouping);
            Suggestions.Remove(grouping);
            Suggestions.Insert(groupingIndex,
                new WorkspaceGroupedCollection<AutocompleteSuggestion>(
                    grouping.WorkspaceName, getSuggestionsWithTasks(grouping)
                )
            );
        }

        private void toggleBillable()
        {
            IsBillable = !IsBillable;
        }

        private async Task changeTime()
        {
            IsEditingTime = true;

            var currentDuration = DurationParameter.WithStartAndDuration(StartTime, Duration);
            var selectedDuration = await navigationService
                .Navigate<EditDurationViewModel, DurationParameter, DurationParameter>(currentDuration)
                .ConfigureAwait(false);

            StartTime = selectedDuration.Start;

            if (selectedDuration.Duration.HasValue)
            {
                Duration = selectedDuration.Duration;
                displayedTime = selectedDuration.Duration.Value;
                elapsedTimeDisposable.Dispose();
                RaisePropertyChanged(nameof(DisplayedTime));
            }

            IsEditingTime = false;
        }

        private async Task setStartDate()
        {
            var minimum = StartTime + DisplayedTime - TimeSpan.FromHours(MaxTimeEntryDurationInHours);
            var maximum = StartTime + DisplayedTime;
            var parameters = DateTimePickerParameters.WithDates(DateTimePickerMode.DateTime, StartTime, minimum, maximum);

            var selectedDate = await navigationService
                .Navigate<SelectDateTimeViewModel, DateTimePickerParameters, DateTimeOffset>(parameters)
                .ConfigureAwait(false);

            var newStartTime = new DateTimeOffset(
                selectedDate.Year,
                selectedDate.Month,
                selectedDate.Day,
                StartTime.Hour,
                StartTime.Minute,
                StartTime.Second,
                StartTime.Offset)
                .Clamp(minimum, maximum);

            if (StartTime.Date != newStartTime.Date)
            {
                StartTime = newStartTime;
            }
        }

        private async Task back()
        {
            if (IsDirty)
            {
                var result = await dialogService.ShowMultipleChoiceDialog(
                    Resources.ContinueEditing,
                    new MultipleChoiceDialogAction(Resources.Discard, true)
                );

                if (result != Resources.Discard)
                    return;
            }

            await navigationService.Close(this);
        }

        private async Task done()
        {
            await dataSource.User.Current
                .Select(user => new StartTimeEntryDTO
                {
                    TaskId = TextFieldInfo.TaskId,
                    StartTime = StartTime,
                    Duration = Duration,
                    Billable = IsBillable,
                    UserId = user.Id,
                    WorkspaceId = TextFieldInfo.WorkspaceId ?? user.DefaultWorkspaceId,
                    Description = TextFieldInfo.Text?.Trim() ?? "",
                    ProjectId = TextFieldInfo.ProjectId,
                    TagIds = TextFieldInfo.Tags.Select(t => t.TagId).Distinct().ToArray()
                })
                .SelectMany(dataSource.TimeEntries.Start)
                .Do(_ => dataSource.SyncManager.PushSync());

            await navigationService.Close(this);
        }

        private bool shouldUpdateSuggestions(TextFieldInfo textFieldInfo)
        {
            if (textFieldInfo.Text != previousTextFieldInfo.Text || textFieldInfo.CursorPosition >= previousTextFieldInfo.CursorPosition)
            {
                previousTextFieldInfo = TextFieldInfo;
                return true;
            }

            return false;
        }

        private void onParsedQuery(QueryInfo parsedQuery)
        {
            CurrentQuery = parsedQuery.Text?.Trim() ?? "";
            IsSuggestingTags = parsedQuery.SuggestionType == AutocompleteSuggestionType.Tags;
            IsSuggestingProjects = parsedQuery.SuggestionType == AutocompleteSuggestionType.Projects;
        }

        private void onSuggestions(IEnumerable<AutocompleteSuggestion> suggestions)
        {
            Suggestions.Clear();

            var filteredSuggestions = filterSuggestions(suggestions);
            var groupedSuggestions = groupSuggestions(filteredSuggestions).ToList();

            UseGrouping = groupedSuggestions.Count > 1;
            Suggestions.AddRange(groupedSuggestions);

            RaisePropertyChanged(nameof(SuggestCreation));
        }

        private IEnumerable<AutocompleteSuggestion> filterSuggestions(IEnumerable<AutocompleteSuggestion> suggestions)
        {
            if (TextFieldInfo.ProjectId.HasValue && !IsSuggestingProjects && !IsSuggestingTags)
            {
                return suggestions.OfType<TimeEntrySuggestion>()
                    .Where(suggestion => suggestion.ProjectId == TextFieldInfo.ProjectId.Value);
            }
            
            return suggestions;
        }

        private IEnumerable<WorkspaceGroupedCollection<AutocompleteSuggestion>> groupSuggestions(
            IEnumerable<AutocompleteSuggestion> suggestions)
        {
            var firstSuggestion = suggestions.FirstOrDefault();
            if (firstSuggestion is ProjectSuggestion)
                return suggestions.GroupByWorkspaceAddingNoProject();

            if (IsSuggestingTags)
                suggestions = suggestions.Where(suggestion => suggestion.WorkspaceId == TextFieldInfo.WorkspaceId);

            return suggestions
                .GroupBy(suggestion => suggestion.WorkspaceName)
                .Select(grouping => new WorkspaceGroupedCollection<AutocompleteSuggestion>(
                    grouping.Key, grouping.Distinct(AutocompleteSuggestionComparer.Instance)));
        }

        private async Task setBillableValues(long projectId)
        {
            var workspaceObservable = projectId == 0
                ? dataSource.Workspaces.GetDefault().Select(ws => (Workspace: ws, DefaultToBillable: false))
                : dataSource.Projects.GetById(projectId)
                    .SelectMany(project =>
                        dataSource.Workspaces
                            .GetById(project.WorkspaceId)
                            .Select(ws => (Workspace: ws, DefaultToBillable: project.Billable ?? false)));

            (IsBillableAvailable, IsBillable) =
                await workspaceObservable
                    .SelectMany(tuple =>
                        dataSource.Workspaces
                            .WorkspaceHasFeature(tuple.Workspace.Id, WorkspaceFeatureId.Pro)
                            .Select(isAvailable => (IsBillableAvailable: isAvailable, IsBillable: isAvailable && tuple.DefaultToBillable)));
        }

        private IEnumerable<AutocompleteSuggestion> getSuggestionsWithTasks(
            IEnumerable<AutocompleteSuggestion> suggestions)
        {
            foreach (var suggestion in suggestions)
            {
                if (suggestion is TaskSuggestion) continue;

                yield return suggestion;

                if (suggestion is ProjectSuggestion projectSuggestion && projectSuggestion.TasksVisible)
                    foreach (var taskSuggestion in projectSuggestion.Tasks)
                        yield return taskSuggestion;
            }
        }
    }
}
