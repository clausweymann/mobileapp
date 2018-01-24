using Android.App;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Webkit;
using MvvmCross.Droid.Support.V7.AppCompat;
using MvvmCross.Droid.Views.Attributes;
using Toggl.Foundation.MvvmCross.ViewModels;
using static Android.Support.V7.Widget.Toolbar;

namespace Toggl.Giskard.Activities
{
    [MvxActivityPresentation]
    [Activity(Theme = "@style/AppTheme")]
    public sealed class BrowserActivity : MvxAppCompatActivity<BrowserViewModel>
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.BrowserActivity);

            OverridePendingTransition(Resource.Animation.abc_fade_in, Resource.Animation.abc_fade_out);

            setupToolbar();
            setupBrowser();
        }

        private void setupToolbar()
        {
            var toolbar = FindViewById<Toolbar>(Resource.Id.Toolbar);

            toolbar.Title = ViewModel.Title;

            SetSupportActionBar(toolbar);

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);

            toolbar.NavigationClick += onNavigateBack;
        }

        private void setupBrowser()
        {
            var webView = FindViewById<WebView>(Resource.Id.BrowserWebView);
            webView.SetWebChromeClient(new WebChromeClient());
            webView.Settings.JavaScriptEnabled = true;
            webView.LoadUrl(ViewModel.Url);
        }

        private void onNavigateBack(object sender, NavigationClickEventArgs e)
        {
            ViewModel.BackCommand.Execute();
        }
    }
}
