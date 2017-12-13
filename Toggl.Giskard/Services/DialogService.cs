using System.Threading.Tasks;
using Toggl.Foundation.MvvmCross.Services;

namespace Toggl.Giskard.Services
{
    public sealed class DialogService : IDialogService
    {
        public Task<bool> Confirm(string title, string message, string confirmButtonText, string dismissButtonText)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> ShowMultipleChoiceDialog(string cancelText, params MultipleChoiceDialogAction[] actions)
        {
            throw new System.NotImplementedException();
        }
    }
}
