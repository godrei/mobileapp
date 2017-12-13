namespace Toggl.PrimeRadiant.Settings
{
    public interface IOnboardingStorage
    {
        void SetIsNewUser(bool isNewUser);
        void SetCompletedOnboarding();

        bool IsNewUser();
        bool CompletedOnboarding();
    }
}
