﻿using Android.App;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using MvvmCross.Droid.Support.V7.AppCompat;
using MvvmCross.Droid.Views.Attributes;
using Toggl.Foundation.MvvmCross.ViewModels;
using static Android.Support.V7.Widget.Toolbar;
using AndroidTextView = Android.Widget.TextView;

namespace Toggl.Giskard.Activities
{
    [Activity(Theme = "@style/Theme.AppCompat.Light.NoActionBar", WindowSoftInputMode = SoftInput.AdjustResize)]
    [MvxActivityPresentation]
    public class LoginActivity : MvxAppCompatActivity<LoginViewModel>
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.LoginActivity);

            setupToolbar();
        }

        private void setupToolbar()
        {
            var toolbar = FindViewById<Toolbar>(Resource.Id.LoginToolbar);

            toolbar.Title = ViewModel.Title;

            SetSupportActionBar(toolbar);

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);

            toolbar.NavigationClick += navigationClick;
        }

        private void navigationClick(object sender, NavigationClickEventArgs args)
        {
            if (ViewModel.IsLoading) return;

            ViewModel.BackCommand.Execute();
        }
    }
}
