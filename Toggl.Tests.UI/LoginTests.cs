using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Xamarin.UITest;
using Toggl.Tests.UI.Helpers;
using static Toggl.Tests.UI.Extensions.LoginExtensions;

namespace Toggl.Tests.UI
{
    [TestFixture]
    public sealed class LoginTests
    {
        private const string validEmail = "susancalvin@psychohistorian.museum";

        private IApp app;

        [SetUp]
        public void BeforeEachTest()
        {
            app = Configuration.GetApp();

            app.WaitForLoginScreen();
        }

        [Test]
        public void TheNextButtonShowsThePasswordField()
        {
            app.EnterText(validEmail);

            app.GoToPasswordScreen();

            app.Screenshot("Login password page.");
        }

        [Test]
        public void TheBackButtonClosesTheLoginViewIfTheEmailFieldIsVisible()
        {
            app.GoBackToOnboardingScreen();

            app.Screenshot("Onboarding last page.");
        }

        [Test]
        public void TheBackButtonShowsTheEmailFieldIfThePasswordFieldIsVisible()
        {
            app.EnterText(validEmail);
            app.GoToPasswordScreen();

            app.GoBackToEmailScreen();

            app.Screenshot("Login email page.");
        }

        [Test]
        public void TheNextButtonAfterInputtingAnInvalidPasswordShowsTheErrorLabel()
        {
            var email = randomEmail();
            app.EnterText(email);
            app.GoToPasswordScreen();

            var password = "asdads";
            app.EnterText($"{password}123456");
            app.TryLoginAndFail();

            app.Screenshot("Login email page.");
        }

        [Test]
        public async Task TheNextButtonAfterInputtingAValidPasswordShowsTheMainScreen()
        {
            var email = randomEmail();
            app.EnterText(email);
            app.GoToPasswordScreen();

            var password = await User.Create(email);
            app.EnterText(password);
            app.LoginSuccesfully();

            app.Screenshot("Login email page.");
        }

        private string randomEmail()
            => $"{Guid.NewGuid().ToString()}@toggl.space";
    }
}
