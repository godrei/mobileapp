using Android.App;
using Android.OS;
using MvvmCross.Droid.Views;

namespace Toggl.Giskard.Activities
{
    [Activity(Label = "View for MainViewModel")]
    public class MainActivity : MvxActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.MainActivity);
        }
    }
}
