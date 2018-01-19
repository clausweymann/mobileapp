// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Toggl.Daneel.ViewControllers
{
	[Register ("MainViewController")]
	partial class MainViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIView CurrentTimeEntryCard { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UILabel CurrentTimeEntryDescriptionLabel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UILabel CurrentTimeEntryElapsedTimeLabel { get; set; }

		[Outlet]
		UIKit.UILabel CurrentTimeEntryProjectTaskClientLabel { get; set; }

		[Outlet]
		UIKit.UIImageView DismissSyncBarImageView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIButton EditTimeEntryButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		Toggl.Daneel.Views.MainScrollView MainPagedScrollView { get; set; }

		[Outlet]
		Toggl.Daneel.Views.FadeView RunningEntryDescriptionFadeView { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint ScrollViewTopConstraint { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		Toggl.Daneel.Views.SpiderOnARopeView SpiderBroView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIView SpiderHinge { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIButton StartTimeEntryButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIButton StopTimeEntryButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIView SuggestionsContainer { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		Toggl.Daneel.Views.ActivityIndicatorView SyncIndicatorView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UILabel SyncStateLabel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIView SyncStateView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIView TimeEntriesLogContainer { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIView TimelineView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (DismissSyncBarImageView != null) {
				DismissSyncBarImageView.Dispose ();
				DismissSyncBarImageView = null;
			}

			if (CurrentTimeEntryCard != null) {
				CurrentTimeEntryCard.Dispose ();
				CurrentTimeEntryCard = null;
			}

			if (CurrentTimeEntryDescriptionLabel != null) {
				CurrentTimeEntryDescriptionLabel.Dispose ();
				CurrentTimeEntryDescriptionLabel = null;
			}

			if (CurrentTimeEntryElapsedTimeLabel != null) {
				CurrentTimeEntryElapsedTimeLabel.Dispose ();
				CurrentTimeEntryElapsedTimeLabel = null;
			}

			if (CurrentTimeEntryProjectTaskClientLabel != null) {
				CurrentTimeEntryProjectTaskClientLabel.Dispose ();
				CurrentTimeEntryProjectTaskClientLabel = null;
			}

			if (EditTimeEntryButton != null) {
				EditTimeEntryButton.Dispose ();
				EditTimeEntryButton = null;
			}

			if (MainPagedScrollView != null) {
				MainPagedScrollView.Dispose ();
				MainPagedScrollView = null;
			}

			if (RunningEntryDescriptionFadeView != null) {
				RunningEntryDescriptionFadeView.Dispose ();
				RunningEntryDescriptionFadeView = null;
			}

			if (ScrollViewTopConstraint != null) {
				ScrollViewTopConstraint.Dispose ();
				ScrollViewTopConstraint = null;
			}

			if (SpiderBroView != null) {
				SpiderBroView.Dispose ();
				SpiderBroView = null;
			}

			if (SpiderHinge != null) {
				SpiderHinge.Dispose ();
				SpiderHinge = null;
			}

			if (StartTimeEntryButton != null) {
				StartTimeEntryButton.Dispose ();
				StartTimeEntryButton = null;
			}

			if (StopTimeEntryButton != null) {
				StopTimeEntryButton.Dispose ();
				StopTimeEntryButton = null;
			}

			if (SuggestionsContainer != null) {
				SuggestionsContainer.Dispose ();
				SuggestionsContainer = null;
			}

			if (SyncIndicatorView != null) {
				SyncIndicatorView.Dispose ();
				SyncIndicatorView = null;
			}

			if (SyncStateLabel != null) {
				SyncStateLabel.Dispose ();
				SyncStateLabel = null;
			}

			if (SyncStateView != null) {
				SyncStateView.Dispose ();
				SyncStateView = null;
			}

			if (TimeEntriesLogContainer != null) {
				TimeEntriesLogContainer.Dispose ();
				TimeEntriesLogContainer = null;
			}

			if (TimelineView != null) {
				TimelineView.Dispose ();
				TimelineView = null;
			}
		}
	}
}
