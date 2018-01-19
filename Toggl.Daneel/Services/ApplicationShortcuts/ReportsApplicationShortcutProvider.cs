//using System;
//using System.Reactive;
//using System.Reactive.Disposables;
//using System.Reactive.Linq;
//using System.Reactive.Subjects;
//using Toggl.Foundation;
//using Toggl.Foundation.DataSources;
//using Toggl.Foundation.Login;
//using Toggl.Multivac;
//using Toggl.Multivac.Models;
//using UIKit;

//namespace Toggl.Daneel.Services.ApplicationShortcuts
//{
//    public class ReportsApplicationShortcutProvider : IApplicationShortcutProvider
//    {
//        private readonly CompositeDisposable disposeBag = new CompositeDisposable();
//        private readonly Subject<ShortcutCollection> shortcutsSubject
//            = new Subject<ShortcutCollection>();

//        public string ShortcutType { get; } = nameof(ReportsApplicationShortcutProvider);

//        public IObservable<ShortcutCollection> Shortcuts { get; }

//        public IApplicationShortcutHandler Handler => throw new NotImplementedException();

//        public ReportsApplicationShortcutProvider(
//            ILoginManager loginManager, ITogglDataSource dataSource)
//        {
//            Ensure.Argument.IsNotNull(loginManager, nameof(loginManager));
//            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));

//            Shortcuts = shortcutsSubject.AsObservable();

//            var userLoggedInSubscription = loginManager
//                .UserLoggedIn
//                .Subscribe(onUserLoggedIn);

//            var userLoggedOutSubscription = dataSource
//                .UserLoggedOut
//                .Subscribe(onUserLoggedOut);

//            disposeBag.Add(userLoggedInSubscription);
//            disposeBag.Add(userLoggedOutSubscription);
//        }

//        private void onUserLoggedIn(IUser _)
//        {
//            UIApplication.SharedApplication.InvokeOnMainThread(() =>
//            {
//                var shortcut = new UIMutableApplicationShortcutItem(
//                    ShortcutType,
//                    Resources.AppShortcutReports
//                );
//                shortcut.Icon = UIApplicationShortcutIcon.FromTemplateImageName("icReports");
//                shortcutsSubject.OnNext(new ShortcutCollection(ShortcutType, new[] { shortcut }));
//            });
//        }

//        private void onUserLoggedOut(Unit _)
//            => shortcutsSubject.OnNext(ShortcutCollection.Empty(ShortcutType));
//    }
//}
