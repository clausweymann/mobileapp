using System.Threading.Tasks;
using UIKit;

namespace Toggl.Daneel.Services.ApplicationShortcuts
{
    public interface IApplicationShortcutHandler
    {
        Task Handle(UIApplicationShortcutItem shortcut);
    }
}
