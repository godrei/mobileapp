using System.Linq;
using System.Threading.Tasks;
using Toggl.Foundation.MvvmCross.Services;
using UIKit;
using static Toggl.Multivac.Extensions.FunctionalExtensions;

namespace Toggl.Daneel.Services
{
    public sealed class DialogService : IDialogService
    {
        public Task<bool> Confirm(
            string title, 
            string message, 
            string confirmButtonText, 
            string dismissButtonText)
        {
            var tcs = new TaskCompletionSource<bool>();

            var alert = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);
            var confirm = UIAlertAction.Create(confirmButtonText, UIAlertActionStyle.Default, _ => tcs.SetResult(true));
            var dismiss = UIAlertAction.Create(dismissButtonText, UIAlertActionStyle.Cancel, _ => tcs.SetResult(false));

            alert.AddAction(confirm);
            alert.AddAction(dismiss);
            alert.PreferredAction = confirm;

            getPresentationController().PresentViewController(alert, true, null);

            return tcs.Task;
        }

        public Task<string> ShowMultipleChoiceDialog(
            string cancelText,
            params MultipleChoiceDialogAction[] actions)
        {
            var tcs = new TaskCompletionSource<string>();
            var actionSheet = UIAlertController.Create(
                title: null,
                message: null,
                preferredStyle: UIAlertControllerStyle.ActionSheet
            );

            var cancelAction = UIAlertAction.Create(cancelText, UIAlertActionStyle.Cancel, _ => tcs.SetResult(cancelText));
            actionSheet.AddAction(cancelAction);

            actions
                .Select(
                    action => UIAlertAction.Create(
                        action.Text,
                        action.Destructive ? UIAlertActionStyle.Destructive : UIAlertActionStyle.Default,
                        _ => tcs.SetResult(action.Text)))
                .ForEach(actionSheet.AddAction);

            getPresentationController().PresentViewController(actionSheet, true, null);

            return tcs.Task;
        }

        private UIViewController getPresentationController()
        {
            var current = UIApplication.SharedApplication.KeyWindow.RootViewController;
            while (current.PresentedViewController != null)
                current = current.PresentedViewController;
            return current;
        }
    }
}
