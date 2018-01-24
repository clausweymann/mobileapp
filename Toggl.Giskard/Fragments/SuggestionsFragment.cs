using System;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Linq;
using Android.OS;
using Android.Views;
using Android.Widget;
using MvvmCross.Binding.Droid.BindingContext;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Droid.Views.Attributes;
using Toggl.Foundation.MvvmCross.ViewModels;
using Toggl.Giskard.Activities;
using Toggl.Giskard.Extensions;
using Toggl.Giskard.Views;
using TogglResources = Toggl.Foundation.Resources;

namespace Toggl.Giskard.Fragments
{
    [MvxFragmentPresentation(typeof(MainViewModel), Resource.Id.MainSuggestionsContainer)]
    public sealed class SuggestionsFragment : MvxFragment<SuggestionsViewModel>
    {
        private TextView hintTextView;
        private TextView indicatorTextView;
        private int containerHeightInPixels;
        private IDisposable countDisposable;
        private IDisposable currentCardDisposable;

        private int currentSuggestionCard = 1;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            containerHeightInPixels = (int)130.DpToPixels(Activity);

            base.OnCreateView(inflater, container, savedInstanceState);
            var view = this.BindingInflate(Resource.Layout.SuggestionsFragment, null);

            hintTextView = view.FindViewById<TextView>(Resource.Id.SuggestionsHintTextView);
            indicatorTextView = view.FindViewById<TextView>(Resource.Id.SuggestionsIndicatorTextView);

            countDisposable =
                Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                    e => ViewModel.Suggestions.CollectionChanged += e,
                    e => ViewModel.Suggestions.CollectionChanged -= e)
                .Select(args => ViewModel.Suggestions.Count)
                .StartWith(ViewModel.Suggestions.Count)
                .DistinctUntilChanged()
                .Subscribe(onCollectionCountChanged);

            currentCardDisposable =
                view.FindViewById<SuggestionsRecyclerView>(Resource.Id.SuggestionsRecyclerView)
                    .CurrentIndexObservable.Subscribe(onCurrentSuggestionIndexChanged);

            return view;
        }

        public override void OnDestroyView()
        {
            currentCardDisposable?.Dispose();
            countDisposable?.Dispose();
            base.OnDestroyView();
        }

        private void onCurrentSuggestionIndexChanged(int currentIndex)
        {
            currentSuggestionCard = currentIndex;
            
            updateHintText();
        }

        private void onCollectionCountChanged(int itemCount)
        {
            var mainActivity = (MainActivity)Activity;
            var height = itemCount > 0 ? containerHeightInPixels : 0;
            var newParams = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, height);
            mainActivity.SuggestionsContainer.LayoutParameters = newParams;

            updateHintText();
        }

        private void updateHintText()
        {
            var numberOfSuggestions = ViewModel.Suggestions.Count;

            switch (numberOfSuggestions)
            {
                case 0:
                    return;
                
                case 1:
                    hintTextView.Text = TogglResources.WorkingOnThis;
                    indicatorTextView.Visibility = ViewStates.Gone;
                    break;

                default:
                    var indicatorText = $"{currentSuggestionCard} {TogglResources.Of.ToUpper()} {numberOfSuggestions}";
                    hintTextView.Text = TogglResources.WorkingOnThese;
                    indicatorTextView.Visibility = ViewStates.Visible;
                    indicatorTextView.Text = indicatorText;
                    break;
            }

        }
    }
}
