using System.Reactive.Linq;
using System.Threading.Tasks;
using MvvmCross.Core.Navigation;
using Toggl.Daneel.ViewControllers;
using Toggl.Daneel.ViewControllers.Navigation;
using Toggl.Foundation.DataSources;
using Toggl.Foundation.MvvmCross.ViewModels;
using Toggl.Multivac;
using UIKit;

namespace Toggl.Daneel.Services.ApplicationShortcuts
{
    public class ReportsApplicationShortcutHandler : IApplicationShortcutHandler
    {
        private readonly ITopViewControllerProvider topViewControllerProvider;
        private readonly IMvxNavigationService navigationService;
        private readonly ITogglDataSource dataSource;

        public ReportsApplicationShortcutHandler(
            ITopViewControllerProvider topViewControllerProvider,
            IMvxNavigationService navigationService,
            ITogglDataSource dataSource)
        {
            Ensure.Argument.IsNotNull(topViewControllerProvider, nameof(topViewControllerProvider));
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));

            this.topViewControllerProvider = topViewControllerProvider;
            this.navigationService = navigationService;
            this.dataSource = dataSource;
        }

        public async Task Handle(UIApplicationShortcutItem shortcut)
        {
            if (topViewControllerProvider.TopViewController is UINavigationController navigationController
                && navigationController.TopViewController is ReportsViewController)
                return;
            
            var user = await dataSource.User.Current;
            await navigationService.Navigate<ReportsViewModel, long>(user.DefaultWorkspaceId);
        }
    }
}
