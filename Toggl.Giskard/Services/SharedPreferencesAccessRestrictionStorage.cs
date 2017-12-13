using System;
using Android.Content;
using Toggl.Foundation;
using Toggl.Multivac;
using Toggl.PrimeRadiant.Settings;

namespace Toggl.Giskard.Services
{
    public sealed class SharedPreferencesAccessRestrictionStorage : IAccessRestrictionStorage, IOnboardingStorage
    {
        private Version version;
        private ISharedPreferences sharedPreferences;

        public SharedPreferencesAccessRestrictionStorage(ISharedPreferences sharedPreferences, ITimeService timeService, Version version)
        {
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));
            Ensure.Argument.IsNotNull(sharedPreferences, nameof(sharedPreferences));

            this.version = version;
            this.sharedPreferences = sharedPreferences;

            var now = timeService.CurrentDateTime;
            var lastUsed = getString(lastAccessDateKey);
            setString(lastAccessDateKey, now.ToString());

            if (lastUsed == null) return;

            var lastUsedDate = DateTimeOffset.Parse(lastUsed);
            var offset = now - lastUsedDate;
            if (offset < TimeSpan.FromDays(newUserThreshold)) return;

            setBool(isNewUserKey, false);
        }

        private const int newUserThreshold = 60;
        private const string outdatedApiKey = "OutdatedApi";
        private const string outdatedClientKey = "OutdatedClient";
        private const string unauthorizedAccessKey = "UnauthorizedAccessForApiToken";

        private const string isNewUserKey = "IsNewUser";
        private const string lastAccessDateKey = "LastAccessDate";
        private const string completedOnboardingKey = "CompletedOnboarding";


        #region IAccessRestrictionStorage

        public void SetClientOutdated()
        {
            setString(outdatedClientKey, version.ToString());
        }

        public void SetApiOutdated()
        {
            setString(outdatedApiKey, version.ToString());
        }

        public void SetUnauthorizedAccess(string apiToken)
        {
            setString(unauthorizedAccessKey, apiToken);
        }

        public bool IsClientOutdated()
            => isOutdated(outdatedClientKey);

        public bool IsApiOutdated()
            => isOutdated(outdatedApiKey);

        public bool IsUnauthorized(string apiToken)
            => apiToken == getString(unauthorizedAccessKey);

        private bool isOutdated(string key)
        {
            var storedVersion = getStoredVersion(key);
            return storedVersion != null && version <= storedVersion;
        }

        private Version getStoredVersion(string key)
        {
            var stored = getString(key);
            return stored == null ? null : Version.Parse(stored);
        }

        #endregion

        #region IOnboardingStorage

        public void SetIsNewUser(bool isNewUser)
        {
            setBool(isNewUserKey, isNewUser);
        }

        public void SetCompletedOnboarding()
        {
            setBool(completedOnboardingKey, true);
        }

        public bool IsNewUser() => getBool(isNewUserKey);

        public bool CompletedOnboarding() => getBool(completedOnboardingKey);

        #endregion

        private bool getBool(string key)
            => sharedPreferences.GetBoolean(key, false);

        private string getString(string key)
            => sharedPreferences.GetString(key, null);

        private void setBool(string key, bool value)
        {
            var editor = sharedPreferences.Edit();
            editor.PutBoolean(key, value);
            editor.Commit();
        }

        private void setString(string key, string value)
        {
            var editor = sharedPreferences.Edit();
            editor.PutString(key, value);
            editor.Commit();
        }
    }
}
