using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using PropertyChanged;
using Toggl.Foundation.DataSources;
using Toggl.Foundation.Login;
using Toggl.Foundation.MvvmCross.Parameters;
using Toggl.Multivac;
using EmailType = Toggl.Multivac.Email;
using LoginType = Toggl.Foundation.MvvmCross.Parameters.LoginParameter.LoginType;

namespace Toggl.Foundation.MvvmCross.ViewModels
{
    [ImplementPropertyChanged]
    public class LoginViewModel : BaseViewModel<LoginParameter>
    {
        public const int EmailPage = 0;
        public const int PasswordPage = 1;

        private readonly ILoginManager loginManager;
        private readonly IMvxNavigationService navigationService;

        private IDisposable loginDisposable;
        private EmailType email = EmailType.Invalid;

        public string Email { get; set; } = "";

        public string Password { get; set; } = "";

        public int CurrentPage { get; private set; } = EmailPage;

        public bool IsLoading { get; private set; } = false;

        public bool IsPasswordMasked { get; private set; } = true;

        public LoginType LoginType { get; set; }

        public IMvxCommand NextCommand { get; }

        public IMvxCommand BackCommand { get; }

        public IMvxCommand TogglePasswordVisibilityCommand { get; }

        [DependsOn(nameof(CurrentPage))]
        public bool IsEmailPage => CurrentPage == EmailPage;

        [DependsOn(nameof(CurrentPage))]
        public bool IsPasswordPage => CurrentPage == PasswordPage;

        [DependsOn(nameof(CurrentPage), nameof(Email), nameof(Password))]
        public bool NextIsEnabled
            => IsEmailPage ? email.IsValid : (Password.Length > 0 && !IsLoading);

        public LoginViewModel(ILoginManager loginManager, IMvxNavigationService navigationService)
        {
            Ensure.Argument.IsNotNull(loginManager, nameof(loginManager));
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));

            this.loginManager = loginManager;
            this.navigationService = navigationService;

            BackCommand = new MvxCommand(back);
            NextCommand = new MvxCommand(next);
            TogglePasswordVisibilityCommand = new MvxCommand(togglePasswordVisibility);
        }

        public override Task Initialize(LoginParameter parameter)
        {
            LoginType = parameter.Type;
            Title = LoginType == LoginType.Login ? Resources.LoginTitle : Resources.SignUpTitle;

            return base.Initialize();
        }

        private void OnEmailChanged()
            => email = EmailType.FromString(Email);

        private void next()
        {
            if (!NextIsEnabled) return;

            if (IsPasswordPage) login();

            CurrentPage = PasswordPage;
        }

        private void back()
        {
            if (IsEmailPage)
                navigationService.Close(this);

            CurrentPage = EmailPage;
        }

        private void togglePasswordVisibility()
            => IsPasswordMasked = !IsPasswordMasked;

        private void login()
        {
            IsLoading = true;
            
            loginDisposable = 
                loginManager
                    .Login(email, Password)
                    .Subscribe(onDataSource, onError, onCompleted);
        }

        private void onDataSource(ITogglDataSource dataSource)
        {
            Mvx.RegisterSingleton(dataSource);

            navigationService.Navigate<TimeEntriesViewModel>();
        }

        private void onError(Exception ex)
        {
            IsLoading = false;
            loginDisposable?.Dispose();
            loginDisposable = null;
        }

        private void onCompleted()
        {
            IsLoading = false;
            loginDisposable?.Dispose();
            loginDisposable = null;
        }
    }
}
