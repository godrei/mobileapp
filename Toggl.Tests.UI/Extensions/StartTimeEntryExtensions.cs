using Xamarin.UITest;

namespace Toggl.Tests.UI.Extensions
{
    public static class StartTimeEntryExtensions
    {
        public static void WaitForStartTimeEntryScreen(this IApp app)
        {
            app.WaitForMainScreen();

            app.Tap(Main.StartTimeEntryButton);
            app.WaitForElement(StartTimeEntry.DoneButton);
        }
    }
}
