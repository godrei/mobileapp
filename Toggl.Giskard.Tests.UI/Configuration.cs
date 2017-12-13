using Xamarin.UITest;
using Xamarin.UITest.Android;

namespace Toggl.Tests.UI
{
    public static class Configuration
    {
        public static AndroidApp GetApp()
            => ConfigureApp
                .Android
                .ApkFile("../Debug/Toggl.Giskard.apk")
                .EnableLocalScreenshots()
                .StartApp();
    }
}
