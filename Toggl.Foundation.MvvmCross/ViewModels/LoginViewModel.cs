﻿using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using PropertyChanged;
using Toggl.Foundation.DataSources;
using Toggl.Foundation.Exceptions;
using Toggl.Foundation.Login;
using Toggl.Foundation.MvvmCross.Parameters;
using Toggl.Foundation.MvvmCross.Services;
using Toggl.Foundation.Services;
using Toggl.Multivac;
using Toggl.PrimeRadiant.Settings;
using Toggl.Ultrawave.Exceptions;
using EmailType = Toggl.Multivac.Email;

namespace Toggl.Foundation.MvvmCross.ViewModels
{
    [Preserve(AllMembers = true)]
    public sealed class LoginViewModel : MvxViewModel<LoginType>
    {
        public const int EmailPage = 0;
        public const int PasswordPage = 1;
        public const int ForgotPasswordPage = 2;
        public const string PrivacyPolicyUrl = "https://toggl.com/legal/privacy";
        public const string TermsOfServiceUrl = "https://toggl.com/legal/terms";

        private readonly ILoginManager loginManager;
        private readonly IOnboardingStorage onboardingStorage;
        private readonly IMvxNavigationService navigationService;
        private readonly IPasswordManagerService passwordManagerService;
        private readonly IApiErrorHandlingService apiErrorHandlingService;

        private LoginType loginType;
        private IDisposable loginDisposable;

        private EmailType email = EmailType.Invalid;

        private int pageBeforeForgotPasswordPage;

        public bool IsLogin => loginType == LoginType.Login;

        public bool IsSignUp => loginType == LoginType.SignUp;

        [DependsOn(nameof(IsLogin), nameof(IsForgotPasswordPage))]
        public string Title
        {
            get
            {
                if (IsSignUp)
                    return Resources.SignUpTitle;

                if (IsForgotPasswordPage)
                    return Resources.LoginForgotPassword;

                return Resources.LoginTitle;
            }
        }

        public string Email { get; set; } = "";

        public string Password { get; set; } = "";

        public string InfoText { get; set; } = "";

        [DependsOn(nameof(InfoText))]
        public bool HasInfoText => !string.IsNullOrEmpty(InfoText);

        public int CurrentPage { get; private set; } = EmailPage;

        public bool IsLoading { get; private set; } = false;

        public bool IsPasswordMasked { get; private set; } = true;

        public bool PasswordManagerVisible
            => IsPasswordManagerAvailable && IsEmailPage && !IsLoading;

        public bool IsEmailFocused
            => CurrentPage == EmailPage || CurrentPage == ForgotPasswordPage;

        public IMvxCommand NextCommand { get; }

        public IMvxCommand BackCommand { get; }

        public IMvxCommand GoogleLoginCommand { get; }

        public IMvxCommand ForgotPasswordCommand { get; }

        public IMvxCommand OpenPrivacyPolicyCommand { get; }

        public IMvxCommand OpenTermsOfServiceCommand { get; }

        public IMvxCommand TogglePasswordVisibilityCommand { get; }

        public IMvxAsyncCommand StartPasswordManagerCommand { get; }

        [DependsOn(nameof(CurrentPage))]
        public bool IsEmailPage => CurrentPage == EmailPage;

        [DependsOn(nameof(CurrentPage))]
        public bool IsPasswordPage => CurrentPage == PasswordPage;

        [DependsOn(nameof(CurrentPage))]
        public bool IsForgotPasswordPage => CurrentPage == ForgotPasswordPage;

        [DependsOn(nameof(IsEmailPage), nameof(IsForgotPasswordPage))]
        public bool EmailFieldVisible => IsEmailPage || IsForgotPasswordPage;

        [DependsOn(nameof(IsPasswordPage), nameof(IsLoading))]
        public bool ShowPasswordButtonVisible => IsPasswordPage && !IsLoading;

        [DependsOn(nameof(IsLogin), nameof(IsForgotPasswordPage))]
        public bool ShowForgotPassword => IsLogin && !IsForgotPasswordPage;

        [DependsOn(nameof(CurrentPage), nameof(Password))]
        public bool NextIsEnabled
        {
            get
            {
                if (IsEmailPage)
                    return email.IsValid;
                if (IsPasswordPage)
                    return Password.Length > 0 && !IsLoading;
                if (IsForgotPasswordPage)
                    return email.IsValid && !IsLoading;
                return false;
            }
        }

        public bool IsPasswordManagerAvailable
            => passwordManagerService.IsAvailable;

        public LoginViewModel(
            ILoginManager loginManager,
            IOnboardingStorage onboardingStorage,
            IMvxNavigationService navigationService,
            IPasswordManagerService passwordManagerService,
            IApiErrorHandlingService apiErrorHandlingService)
        {
            Ensure.Argument.IsNotNull(loginManager, nameof(loginManager));
            Ensure.Argument.IsNotNull(onboardingStorage, nameof(onboardingStorage));
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));
            Ensure.Argument.IsNotNull(passwordManagerService, nameof(passwordManagerService));
            Ensure.Argument.IsNotNull(apiErrorHandlingService, nameof(apiErrorHandlingService));

            this.loginManager = loginManager;
            this.onboardingStorage = onboardingStorage;
            this.navigationService = navigationService;
            this.passwordManagerService = passwordManagerService;
            this.apiErrorHandlingService = apiErrorHandlingService;

            BackCommand = new MvxCommand(back);
            NextCommand = new MvxCommand(next);
            GoogleLoginCommand = new MvxCommand(googleLogin);
            ForgotPasswordCommand = new MvxCommand(forgotPassword);
            OpenPrivacyPolicyCommand = new MvxCommand(openPrivacyPolicyCommand);
            OpenTermsOfServiceCommand = new MvxCommand(openTermsOfServiceCommand);
            StartPasswordManagerCommand = new MvxAsyncCommand(startPasswordManager);
            TogglePasswordVisibilityCommand = new MvxCommand(togglePasswordVisibility);
        }

        public override void Prepare(LoginType parameter)
        {
            loginType = parameter;
        }

        private void OnEmailChanged()
        {
            email = EmailType.FromString(Email);
            RaisePropertyChanged(nameof(NextIsEnabled));
        }

        private void openTermsOfServiceCommand() =>
            navigationService.Navigate<BrowserViewModel, BrowserParameters>(
                BrowserParameters.WithUrlAndTitle(TermsOfServiceUrl, Resources.TermsOfService)
            );

        private void openPrivacyPolicyCommand() =>
            navigationService.Navigate<BrowserViewModel, BrowserParameters>(
                BrowserParameters.WithUrlAndTitle(PrivacyPolicyUrl, Resources.PrivacyPolicy)
            );

        private void next()
        {
            if (!NextIsEnabled) return;

            if (IsPasswordPage)
            {
                if (IsLogin) login();
                if (IsSignUp) signUp();
            }

            if (IsForgotPasswordPage)
            {
                resetPassword();
                return;
            }

            CurrentPage = PasswordPage;
            InfoText = loginType == LoginType.SignUp
                ? Resources.SignUpPasswordRequirements
                : "";
        }

        private void resetPassword()
        {
            IsLoading = true;
            loginManager
                .ResetPassword(email)
                .Subscribe(onPasswordResetSuccess, onPasswordResetError);
        }

        private void onPasswordResetSuccess(string result)
        {
            IsLoading = false;
            CurrentPage = PasswordPage;
            InfoText = Resources.PasswordResetSuccess;
        }

        private void onPasswordResetError(Exception exception)
        {
            IsLoading = false;

            switch (exception)
            {
                case BadRequestException _:
                    InfoText = Resources.PasswordResetEmailDoesNotExistError;
                    break;

                case OfflineException _:
                    InfoText = Resources.PasswordResetOfflineError;
                    break;

                case ApiException apiException:
                    InfoText = apiException.LocalizedApiErrorMessage;
                    break;

                default:
                    InfoText = Resources.PasswordResetGeneralError;
                    break;
            }
        }

        private void back()
        {
            if (IsEmailPage)
                navigationService.Close(this);

            if (IsForgotPasswordPage)
                CurrentPage = pageBeforeForgotPasswordPage;
            else
                CurrentPage--;
            
            InfoText = "";
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

        private void googleLogin()
        {
            if (IsLoading) return;

            IsLoading = true;

            loginDisposable =
                loginManager
                    .LoginWithGoogle()
                    .Subscribe(onDataSource, onError, onCompleted);
        }

        private void signUp()
        {
            IsLoading = true;

            loginDisposable =
                loginManager
                    .SignUp(email, Password)
                    .Subscribe(onDataSource, onError, onCompleted);
        }

        private async Task startPasswordManager()
        {
            if (!passwordManagerService.IsAvailable) return;

            var loginInfo = await passwordManagerService.GetLoginInformation();

            Email = loginInfo.Email;
            if (!NextIsEnabled) return;
            
            next();
            Password = loginInfo.Password;
            if (!NextIsEnabled) return;
            
            next();
        }

        private async void onDataSource(ITogglDataSource dataSource)
        {
            await dataSource.SyncManager.ForceFullSync();

            IsLoading = false;

            onboardingStorage.SetIsNewUser(IsSignUp);

            await navigationService.Navigate<MainViewModel>();
        }

        private void onError(Exception exception)
        {
            IsLoading = false;
            onCompleted();

            if (apiErrorHandlingService.TryHandleDeprecationError(exception))
                return;
                
            switch (exception)
            {
                case UnauthorizedException forbidden:
                    InfoText = Resources.IncorrectEmailOrPassword;
                    break;
                case GoogleLoginException googleEx when googleEx.LoginWasCanceled:
                    InfoText = "";
                    break;
                default:
                    InfoText = getGenericError();
                    break;
            }
        }

        private string getGenericError()
            => loginType == LoginType.Login ? Resources.GenericLoginError : Resources.GenericSignUpError;

        private void onCompleted()
        {
            loginDisposable?.Dispose();
            loginDisposable = null;
        }

        private void forgotPassword()
        {
            pageBeforeForgotPasswordPage = CurrentPage;
            CurrentPage = ForgotPasswordPage;
            InfoText = email.IsValid ? "" : Resources.PasswordResetExplanation;
        }
    }
}
