using System.Reflection;
using Android.App;
using Android.OS;
using Xunit.Sdk;
using Xunit.Runners.UI;
using Xunit;
using FluentAssertions;

namespace Toggl.Giskard.Tests
{
    [Activity(Label = "xUnit Android Runner", MainLauncher = true, Theme = "@android:style/Theme.Material.Light")]
    public class MainActivity : RunnerActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            AddTestAssembly(Assembly.GetExecutingAssembly());
            AddExecutionAssembly(typeof(ExtensibilityPointFactory).Assembly);
            AutoStart = true;
            //TerminateAfterExecution = true;
            base.OnCreate(bundle);
        }
    }
}

