using Android.App;
using Android.OS;
using MvvmCross.Droid.Support.V7.AppCompat;
using MvvmCross.Droid.Views;
using MvvmCross.Droid.Views.Attributes;
using Toggl.Foundation.MvvmCross.ViewModels;

namespace Toggl.Giskard.Activities
{
    [Activity(Theme = "@style/Theme.AppCompat.Light.NoActionBar")]
    [MvxActivityPresentation]
    public class MainActivity : MvxAppCompatActivity<MainViewModel>
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.MainActivity);
        }
    }
}
