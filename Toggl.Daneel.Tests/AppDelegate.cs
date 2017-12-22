using System.Reflection;
using FluentAssertions;
using Foundation;
using UIKit;
using Xunit;
using Xunit.Runner;
using Xunit.Sdk;

namespace Toggl.Daneel.Tests
{
    [Register(nameof(AppDelegate))]
    public partial class AppDelegate : RunnerAppDelegate
    {
        public override bool FinishedLaunching(UIApplication uiApplication, NSDictionary launchOptions)
        {
            AddExecutionAssembly(typeof(ExtensibilityPointFactory).Assembly);
            AddTestAssembly(Assembly.GetExecutingAssembly());
            AutoStart = true;
            //TerminateAfterExecution = true;
            return base.FinishedLaunching(uiApplication, launchOptions);
        }
    }
}