using NUnit.Framework;
using Xamarin.UITest;
using static Toggl.Tests.UI.Extensions.MainExtensions;

namespace Toggl.Tests.UI
{
    [TestFixture]
    public sealed class MainTests
    {
        private const string validEmail = "susancalvin@psychohistorian.museum";

        private IApp app;

        [SetUp]
        public void BeforeEachTest()
        {
            app = Configuration.GetApp();

            app.WaitForMainScreen();
        }

        [Test]
        public void TappingTheStopButtonStopsTheRunningTimeEntry()
        {
            app.StartTimeEntryWithDescription("Testing the Toggl app");

            app.Tap(Main.StopTimeEntryButton);
            app.WaitForElement(Main.StartTimeEntryButton);
        }
    }
}
