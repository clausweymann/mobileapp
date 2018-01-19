using System;
using Toggl.Foundation.DataSources;
using Toggl.Multivac;
using Toggl.Multivac.Models;

namespace Toggl.Foundation.Login
{
    public interface ILoginManager
    {
        ITogglDataSource GetDataSourceIfLoggedIn();

        IObservable<ITogglDataSource> LoginWithGoogle();
        IObservable<ITogglDataSource> Login(Email email, Password password);

        IObservable<ITogglDataSource> SignUpWithGoogle();
        IObservable<ITogglDataSource> SignUp(Email email, Password password);

        IObservable<ITogglDataSource> RefreshToken(Password password);

        IObservable<string> ResetPassword(Email email);

        IObservable<IUser> UserLoggedIn { get; }
    }
}
