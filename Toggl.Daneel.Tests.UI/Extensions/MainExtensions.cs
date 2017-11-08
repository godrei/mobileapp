using System;
using Xamarin.UITest;
using Toggl.Daneel.Tests.UI.Helpers;

namespace Toggl.Daneel.Tests.UI.Extensions
{
    public static class MainExtensions
    {
        public static void WaitForMainScreen(this IApp app)
        {
            var email = $"{Guid.NewGuid().ToString()}@toggl.space";
            var task = User.Create(email);

            app.WaitForLoginScreen();

            app.EnterText(email);
            app.GoToPasswordScreen();

            task.Wait();
            var password = task.Result;

            app.EnterText(password);
            app.LoginSuccesfully();
        }

        public static void StartTimeEntryWithDescription(this IApp app, string description)
        {
            app.Tap(Main.StartTimeEntryButton);
            app.WaitForElement(StartTimeEntry.DoneButton);

            app.EnterText(description);
            app.Tap(StartTimeEntry.DoneButton);

            app.WaitForElement(Main.StopTimeEntryButton);
        }
    }
}
