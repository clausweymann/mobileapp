﻿using System;
using CoreGraphics;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Binding.iOS;
using MvvmCross.Core.ViewModels;
using MvvmCross.iOS.Views;
using MvvmCross.Plugins.Color.iOS;
using MvvmCross.Plugins.Visibility;
using Toggl.Daneel.Combiners;
using Toggl.Daneel.Extensions;
using Toggl.Daneel.Presentation.Attributes;
using Toggl.Daneel.Presentation.Transition;
using Toggl.Foundation;
using Toggl.Foundation.MvvmCross.Converters;
using Toggl.Foundation.MvvmCross.Helper;
using Toggl.Foundation.MvvmCross.ViewModels;
using UIKit;

namespace Toggl.Daneel.ViewControllers
{
    [ModalCardPresentation]
    public partial class EditTimeEntryViewController : MvxViewController<EditTimeEntryViewModel>
    {
        private const float nonScrollableContentHeight = 116f;

        private EditTimeEntryErrorView syncErrorMessageView;

        public EditTimeEntryViewController() : base(nameof(EditTimeEntryViewController), null)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            prepareViews();

            if (PresentationController is ModalPresentationController modalPresentationController)
            {
                syncErrorMessageView = EditTimeEntryErrorView.Create();
                var contentView = modalPresentationController.AdditionalContentView;

                contentView.AddSubview(syncErrorMessageView);

                syncErrorMessageView.TranslatesAutoresizingMaskIntoConstraints = false;
                syncErrorMessageView.TopAnchor
                    .ConstraintEqualTo(contentView.TopAnchor, 28).Active = true;
                syncErrorMessageView.LeadingAnchor
                    .ConstraintEqualTo(contentView.LeadingAnchor, 8).Active = true;
                syncErrorMessageView.TrailingAnchor
                    .ConstraintEqualTo(contentView.TrailingAnchor, -8).Active = true;
            }

            var durationConverter = new TimeSpanToDurationWithUnitValueConverter();
            var dateConverter = new DateToTitleStringValueConverter();
            var timeConverter = new DateTimeToTimeValueConverter();
            var visibilityConverter = new MvxVisibilityValueConverter();
            var invertedBoolConverter = new BoolToConstantValueConverter<bool>(false, true);
            var inverterVisibilityConverter = new MvxInvertedVisibilityValueConverter();
            var projectTaskClientCombiner = new ProjectTaskClientValueCombiner(
                ProjectTaskClientLabel.Font.CapHeight,
                Color.EditTimeEntry.ClientText.ToNativeColor(),
                false
            );
            var stopRunningTimeEntryAndEditDurationForStoppedConverter = new BoolToConstantValueConverter<IMvxCommand>(
                ViewModel.StopCommand, ViewModel.EditDurationCommand);

            var bindingSet = this.CreateBindingSet<EditTimeEntryViewController, EditTimeEntryViewModel>();

            if (syncErrorMessageView != null)
            {
                bindingSet.Bind(syncErrorMessageView)
                          .For(v => v.Text)
                          .To(vm => vm.SyncErrorMessage);

                bindingSet.Bind(syncErrorMessageView)
                          .For(v => v.BindTap())
                          .To(vm => vm.DismissSyncErrorMessageCommand);

                bindingSet.Bind(syncErrorMessageView)
                          .For(v => v.CloseCommand)
                          .To(vm => vm.DismissSyncErrorMessageCommand);

                bindingSet.Bind(syncErrorMessageView)
                          .For(v => v.BindVisible())
                          .To(vm => vm.SyncErrorMessageVisible)
                          .WithConversion(inverterVisibilityConverter);
            }

            //Text
            bindingSet.Bind(DescriptionTextView)
                      .For(v => v.RemainingLength)
                      .To(vm => vm.DescriptionRemainingLength);

            bindingSet.Bind(DescriptionTextView)
                      .For(v => v.BindText())
                      .To(vm => vm.Description);

            bindingSet.Bind(BillableSwitch)
                      .For(v => v.BindAnimatedOn())
                      .To(vm => vm.Billable);

            bindingSet.Bind(DurationLabel)
                      .To(vm => vm.Duration)
                      .WithConversion(durationConverter);

            bindingSet.Bind(ProjectTaskClientLabel)
                      .For(v => v.AttributedText)
                      .ByCombining(projectTaskClientCombiner,
                          v => v.Project,
                          v => v.Task,
                          v => v.Client,
                          v => v.ProjectColor);

            bindingSet.Bind(StartDateLabel)
                      .To(vm => vm.StartTime)
                      .WithConversion(dateConverter);

            bindingSet.Bind(StartTimeLabel)
                      .To(vm => vm.StartTime)
                      .WithConversion(timeConverter);

            bindingSet.Bind(EndTimeLabel)
                      .To(vm => vm.StopTime)
                      .WithConversion(timeConverter);

            //Commands
            bindingSet.Bind(CloseButton).To(vm => vm.CloseCommand);
            bindingSet.Bind(DeleteButton).To(vm => vm.DeleteCommand);
            bindingSet.Bind(ConfirmButton).To(vm => vm.ConfirmCommand);

            bindingSet.Bind(DurationView)
                      .For(v => v.BindTap())
                      .To(vm => vm.EditDurationCommand);

            bindingSet.Bind(StartTimeView)
                      .For(v => v.BindTap())
                      .To(vm => vm.EditDurationCommand);

            bindingSet.Bind(StopButton)
                      .To(vm => vm.StopCommand);

            bindingSet.Bind(EndTimeView)
                      .For(v => v.BindTap())
                      .To(vm => vm.IsTimeEntryRunning)
                      .WithConversion(stopRunningTimeEntryAndEditDurationForStoppedConverter);

            bindingSet.Bind(ProjectTaskClientLabel)
                      .For(v => v.BindTap())
                      .To(vm => vm.SelectProjectCommand);

            bindingSet.Bind(AddProjectAndTaskView)
                      .For(v => v.BindTap())
                      .To(vm => vm.SelectProjectCommand);

            bindingSet.Bind(StartDateView)
                      .For(v => v.BindTap())
                      .To(vm => vm.SelectStartDateCommand);

            bindingSet.Bind(TagsTextView)
                      .For(v => v.BindTap())
                      .To(vm => vm.SelectTagsCommand);

            bindingSet.Bind(AddTagsView)
                      .For(v => v.BindTap())
                      .To(vm => vm.SelectTagsCommand);

            bindingSet.Bind(BillableView)
                      .For(v => v.BindTap())
                      .To(vm => vm.ToggleBillableCommand);

            //End time and the stop button visibility
            bindingSet.Bind(StopButton)
                      .For(v => v.BindVisible())
                      .To(vm => vm.IsTimeEntryRunning)
                      .WithConversion(inverterVisibilityConverter);

            bindingSet.Bind(EndTimeLabel)
                      .For(v => v.BindVisible())
                      .To(vm => vm.IsTimeEntryRunning)
                      .WithConversion(visibilityConverter);

            //Project visibility
            bindingSet.Bind(AddProjectAndTaskView)
                      .For(v => v.BindVisible())
                      .To(vm => vm.Project)
                      .WithConversion(visibilityConverter);

            bindingSet.Bind(ProjectTaskClientLabel)
                      .For(v => v.BindVisible())
                      .To(vm => vm.Project)
                      .WithConversion(inverterVisibilityConverter);

            //Tags visibility
            bindingSet.Bind(AddTagsView)
                      .For(v => v.BindVisible())
                      .To(vm => vm.HasTags)
                      .WithConversion(visibilityConverter);

            bindingSet.Bind(TagsTextView)
                      .For(v => v.BindVisible())
                      .To(vm => vm.HasTags)
                      .WithConversion(inverterVisibilityConverter);

            //Confirm button enabled
            bindingSet.Bind(ConfirmButton)
                      .For(v => v.Enabled)
                      .To(vm => vm.DescriptionLimitExceeded)
                      .WithConversion(invertedBoolConverter);

            bindingSet.Bind(ConfirmButton)
                      .For(v => v.Alpha)
                      .To(vm => vm.DescriptionLimitExceeded)
                      .WithConversion(new BoolToConstantValueConverter<nfloat>(0.5f, 1));

            bindingSet.Apply();
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();
            
            //This binding needs to be created, when TagsTextView has it's
            //proper size. In ViewDidLoad() TagsTextView width isn't initialized
            //yet, which results in displaying less tags than possible
            this.CreateBinding(TagsTextView)
                .For(v => v.BindTags())
                .To<EditTimeEntryViewModel>(vm => vm.Tags)
                .Apply();
        }

        public override void ViewWillLayoutSubviews()
        {
            var newSize = new CGSize(0, nonScrollableContentHeight + ScrollViewContent.Bounds.Height);
            if (newSize != PreferredContentSize)
            {
                PreferredContentSize = newSize;
                PresentationController.ContainerViewWillLayoutSubviews();
                ScrollView.ScrollEnabled = ScrollViewContent.Bounds.Height > ScrollView.Bounds.Height;
            }
        }

        private void prepareViews()
        {
            DurationLabel.Font = DurationLabel.Font.GetMonospacedDigitFont();
            PreferredContentSize = View.Frame.Size;
            BillableSwitch.Resize();
            prepareDescriptionField();
            centerTextVertically(TagsTextView);
            TagsTextView.TextContainer.LineFragmentPadding = 0;

            if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
            {
                var bottomSafeAreaInset = UIApplication.SharedApplication.KeyWindow.SafeAreaInsets.Bottom;
                if (bottomSafeAreaInset >= DeleteButtonBottomConstraint.Constant)
                    DeleteButtonBottomConstraint.Constant
                        = ConfirmButtonBottomConstraint.Constant
                        = 0;
            }
        }

        private void prepareDescriptionField()
        {
            DescriptionTextView.TintColor = Color.StartTimeEntry.Cursor.ToNativeColor();
            DescriptionTextView.PlaceholderText = Resources.AddDescription;
        }

        private void centerTextVertically(UITextView textView)
        {
            var topOffset = (textView.Bounds.Height - textView.ContentSize.Height) / 2;
            textView.ContentInset = new UIEdgeInsets(topOffset, 0, 0, 0);
        }
    }
}
